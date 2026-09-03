using System;
using System.Data;
using System.Threading.Tasks;
using Escola.Aplicacao;
using Escola.Aplicacao.Matriculas;
using Escola.Dominio;
using Escola.Dominio.Entidades;
using Escola.Dominio.Excecoes;
using Escola.Dominio.Repositorios;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Escola.Testes.Unitarios.Matriculas
{
    public class MatriculaServiceTests
    {
        private readonly IConnectionFactory _connectionFactory;
        private readonly IDbConnection _connection;
        private readonly IDbTransaction _transaction;
        private readonly IAlunoRepository _alunoRepository;
        private readonly ITurmaRepository _turmaRepository;
        private readonly IMatriculaRepository _matriculaRepository;
        private readonly ICacheService _cacheService;
        private readonly MatriculaService _sut;

        public MatriculaServiceTests()
        {
            _connectionFactory = Substitute.For<IConnectionFactory>();
            _connection = Substitute.For<IDbConnection>();
            _transaction = Substitute.For<IDbTransaction>();
            _alunoRepository = Substitute.For<IAlunoRepository>();
            _turmaRepository = Substitute.For<ITurmaRepository>();
            _matriculaRepository = Substitute.For<IMatriculaRepository>();
            _cacheService = Substitute.For<ICacheService>();

            _connectionFactory.Create().Returns(_connection);
            _connection.BeginTransaction().Returns(_transaction);

            _sut = new MatriculaService(
                _connectionFactory,
                _alunoRepository,
                _turmaRepository,
                _matriculaRepository,
                _cacheService);
        }

        [Fact]
        public async Task CriarAsync_ValidEnrollment_InsertsDecrementsAndInvalidatesCache()
        {
            ArrangeHappyPath();
            var criada = new Matricula { Id = 21, AlunoId = 1, TurmaId = 2, DataMatricula = DateTime.UtcNow };
            _matriculaRepository.InsertAsync(1, 2, _transaction).Returns(criada);

            var result = await _sut.CriarAsync(new CriarMatriculaDto { AlunoId = 1, TurmaId = 2 });

            result.Id.Should().Be(21);
            await _turmaRepository.Received(1).TryDecrementVagasAsync(2, _transaction);
            await _matriculaRepository.Received(1).InsertAsync(1, 2, _transaction);
            _transaction.Received(1).Commit();
            await _cacheService.Received(1).RemoveAsync(CacheKeys.TurmasListagem);
        }

        [Fact]
        public async Task CriarAsync_InactiveAluno_ThrowsConflictAndDoesNotRemoveCache()
        {
            _alunoRepository.GetByIdAsync(4, _transaction).Returns(Aluno(4, ativo: false));

            Func<Task> act = () => _sut.CriarAsync(new CriarMatriculaDto { AlunoId = 4, TurmaId = 1 });

            await act.Should().ThrowAsync<ConflictException>();
            await AssertNoWriteOrCacheAsync();
        }

        [Fact]
        public async Task CriarAsync_NoSeats_ThrowsConflictAndDoesNotRemoveCache()
        {
            _alunoRepository.GetByIdAsync(1, _transaction).Returns(Aluno(1, ativo: true));
            _turmaRepository.GetByIdForUpdateAsync(4, _transaction).Returns(Turma(4, vagas: 0));

            Func<Task> act = () => _sut.CriarAsync(new CriarMatriculaDto { AlunoId = 1, TurmaId = 4 });

            await act.Should().ThrowAsync<ConflictException>();
            await AssertNoWriteOrCacheAsync();
        }

        [Fact]
        public async Task CriarAsync_Duplicate_ThrowsConflictAndDoesNotRemoveCache()
        {
            _alunoRepository.GetByIdAsync(1, _transaction).Returns(Aluno(1, ativo: true));
            _turmaRepository.GetByIdForUpdateAsync(1, _transaction).Returns(Turma(1, vagas: 5));
            _matriculaRepository.ExistsAsync(1, 1, _transaction).Returns(true);

            Func<Task> act = () => _sut.CriarAsync(new CriarMatriculaDto { AlunoId = 1, TurmaId = 1 });

            await act.Should().ThrowAsync<ConflictException>();
            await AssertNoWriteOrCacheAsync();
        }

        [Fact]
        public async Task CriarAsync_MissingAluno_ThrowsNotFoundAndDoesNotRemoveCache()
        {
            _alunoRepository.GetByIdAsync(99, _transaction).Returns((Aluno)null);

            Func<Task> act = () => _sut.CriarAsync(new CriarMatriculaDto { AlunoId = 99, TurmaId = 1 });

            await act.Should().ThrowAsync<NotFoundException>();
            await AssertNoWriteOrCacheAsync();
        }

        [Fact]
        public async Task CriarAsync_MissingTurma_ThrowsNotFoundAndDoesNotRemoveCache()
        {
            _alunoRepository.GetByIdAsync(1, _transaction).Returns(Aluno(1, ativo: true));
            _turmaRepository.GetByIdForUpdateAsync(99, _transaction).Returns((Turma)null);

            Func<Task> act = () => _sut.CriarAsync(new CriarMatriculaDto { AlunoId = 1, TurmaId = 99 });

            await act.Should().ThrowAsync<NotFoundException>();
            await AssertNoWriteOrCacheAsync();
        }

        [Fact]
        public async Task CriarAsync_NonPositiveIds_ThrowsValidationException()
        {
            Func<Task> act = () => _sut.CriarAsync(new CriarMatriculaDto { AlunoId = 0, TurmaId = 1 });

            await act.Should().ThrowAsync<ValidationException>();
            _connectionFactory.DidNotReceive().Create();
            await _cacheService.DidNotReceive().RemoveAsync(Arg.Any<string>());
        }

        [Fact]
        public async Task CriarAsync_UniqueIndexConflict_DoesNotRemoveCache()
        {
            ArrangeHappyPath();
            _matriculaRepository.InsertAsync(1, 2, _transaction)
                .Returns(_ => Task.FromException<Matricula>(
                    new ConflictException("Aluno já está matriculado nesta turma.")));

            Func<Task> act = () => _sut.CriarAsync(new CriarMatriculaDto { AlunoId = 1, TurmaId = 2 });

            await act.Should().ThrowAsync<ConflictException>();
            _transaction.DidNotReceive().Commit();
            await _cacheService.DidNotReceive().RemoveAsync(Arg.Any<string>());
        }

        private void ArrangeHappyPath()
        {
            _alunoRepository.GetByIdAsync(1, _transaction).Returns(Aluno(1, ativo: true));
            _turmaRepository.GetByIdForUpdateAsync(2, _transaction).Returns(Turma(2, vagas: 30));
            _matriculaRepository.ExistsAsync(1, 2, _transaction).Returns(false);
            _turmaRepository.TryDecrementVagasAsync(2, _transaction).Returns(1);
        }

        private async Task AssertNoWriteOrCacheAsync()
        {
            await _turmaRepository.DidNotReceive().TryDecrementVagasAsync(Arg.Any<int>(), Arg.Any<IDbTransaction>());
            await _matriculaRepository.DidNotReceive().InsertAsync(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<IDbTransaction>());
            _transaction.DidNotReceive().Commit();
            await _cacheService.DidNotReceive().RemoveAsync(Arg.Any<string>());
        }

        private static Aluno Aluno(int id, bool ativo)
        {
            return new Aluno { Id = id, Nome = "Aluno", Ativo = ativo };
        }

        private static Turma Turma(int id, int vagas)
        {
            return new Turma { Id = id, Nome = "Turma", VagasDisponiveis = vagas };
        }
    }
}
