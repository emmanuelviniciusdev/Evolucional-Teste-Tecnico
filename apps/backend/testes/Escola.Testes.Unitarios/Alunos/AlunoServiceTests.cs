using System;
using System.Threading.Tasks;
using Escola.Aplicacao.Alunos;
using Escola.Dominio.Entidades;
using Escola.Dominio.Excecoes;
using Escola.Dominio.Repositorios;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Escola.Testes.Unitarios.Alunos
{
    public class AlunoServiceTests
    {
        private readonly IAlunoRepository _repository;
        private readonly AlunoService _sut;

        public AlunoServiceTests()
        {
            _repository = Substitute.For<IAlunoRepository>();
            _sut = new AlunoService(_repository);
        }

        [Theory]
        [InlineData("anasouza2345@email.com")]
        [InlineData("ana.souza@email.com")]
        public async Task CriarAsync_CompleteEmail_PersistsAluno(string email)
        {
            _repository.InsertAsync(Arg.Any<Aluno>()).Returns(9);
            _repository.GetByIdAsync(9).Returns(new Aluno
            {
                Id = 9,
                Nome = "Ana Souza",
                Email = email,
                DataNascimento = new DateTime(2006, 3, 14),
                Ativo = true
            });

            var result = await _sut.CriarAsync(ValidDto(email));

            result.Email.Should().Be(email);
            result.Ativo.Should().BeTrue();
            await _repository.Received(1).InsertAsync(Arg.Is<Aluno>(a => a.Email == email && a.Ativo));
        }

        [Theory]
        [InlineData("a@b")]
        [InlineData("user@localhost")]
        public async Task CriarAsync_IncompleteEmail_ThrowsValidationException(string email)
        {
            Func<Task> act = () => _sut.CriarAsync(ValidDto(email));

            await act.Should().ThrowAsync<ValidationException>();
            await _repository.DidNotReceive().InsertAsync(Arg.Any<Aluno>());
        }

        [Fact]
        public async Task CriarAsync_DataNascimentoWithTime_ThrowsValidationException()
        {
            var dto = ValidDto("ana.souza@email.com");
            dto.DataNascimento = "2006-03-14T00:00:00";

            Func<Task> act = () => _sut.CriarAsync(dto);

            await act.Should().ThrowAsync<ValidationException>();
            await _repository.DidNotReceive().InsertAsync(Arg.Any<Aluno>());
        }

        [Theory]
        [InlineData(0)]
        [InlineData(101)]
        public async Task ListarAsync_InvalidTamanhoPagina_ThrowsValidationException(int tamanhoPagina)
        {
            Func<Task> act = () => _sut.ListarAsync(null, 1, tamanhoPagina);

            await act.Should().ThrowAsync<ValidationException>()
                .WithMessage("tamanhoPagina deve estar entre 1 e 100.");
        }

        private static AlunoEscritaDto ValidDto(string email)
        {
            return new AlunoEscritaDto
            {
                Nome = "Ana Souza",
                Email = email,
                DataNascimento = "2006-03-14"
            };
        }
    }
}
