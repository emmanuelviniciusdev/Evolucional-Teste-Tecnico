using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Escola.Dominio;
using Escola.Dominio.Entidades;
using Escola.Dominio.Excecoes;
using Escola.Dominio.Repositorios;

namespace Escola.Aplicacao.Matriculas
{
    public class MatriculaService
    {
        private readonly IConnectionFactory _connectionFactory;
        private readonly IAlunoRepository _alunoRepository;
        private readonly ITurmaRepository _turmaRepository;
        private readonly IMatriculaRepository _matriculaRepository;
        private readonly ICacheService _cacheService;

        public MatriculaService(
            IConnectionFactory connectionFactory,
            IAlunoRepository alunoRepository,
            ITurmaRepository turmaRepository,
            IMatriculaRepository matriculaRepository,
            ICacheService cacheService)
        {
            _connectionFactory = connectionFactory;
            _alunoRepository = alunoRepository;
            _turmaRepository = turmaRepository;
            _matriculaRepository = matriculaRepository;
            _cacheService = cacheService;
        }

        public async Task<MatriculaDto> CriarAsync(CriarMatriculaDto dto)
        {
            if (dto == null || dto.AlunoId <= 0 || dto.TurmaId <= 0)
            {
                throw new ValidationException("Aluno e turma são obrigatórios.");
            }

            Matricula criada;
            using (var connection = _connectionFactory.Create())
            using (var transaction = connection.BeginTransaction())
            {
                var aluno = await _alunoRepository.GetByIdAsync(dto.AlunoId, transaction).ConfigureAwait(false);
                if (aluno == null)
                {
                    throw new NotFoundException("Aluno não encontrado.");
                }

                if (!aluno.Ativo)
                {
                    throw new ConflictException("Aluno inativo não pode ser matriculado.");
                }

                var turma = await _turmaRepository.GetByIdForUpdateAsync(dto.TurmaId, transaction).ConfigureAwait(false);
                if (turma == null)
                {
                    throw new NotFoundException("Turma não encontrada.");
                }

                if (turma.VagasDisponiveis <= 0)
                {
                    throw new ConflictException("Turma sem vagas disponíveis.");
                }

                var jaMatriculado = await _matriculaRepository.ExistsAsync(dto.AlunoId, dto.TurmaId, transaction)
                    .ConfigureAwait(false);
                if (jaMatriculado)
                {
                    throw new ConflictException("Aluno já está matriculado nesta turma.");
                }

                var atualizados = await _turmaRepository.TryDecrementVagasAsync(dto.TurmaId, transaction)
                    .ConfigureAwait(false);
                if (atualizados == 0)
                {
                    throw new ConflictException("Turma sem vagas disponíveis.");
                }

                criada = await _matriculaRepository.InsertAsync(dto.AlunoId, dto.TurmaId, transaction)
                    .ConfigureAwait(false);
                transaction.Commit();
            }

            await InvalidateTurmaListingCacheAsync().ConfigureAwait(false);
            return ToDto(criada);
        }

        private async Task InvalidateTurmaListingCacheAsync()
        {
            try
            {
                await _cacheService.RemoveAsync(CacheKeys.TurmasListagem).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Trace.TraceWarning("Failed to invalidate turma listing cache: {0}", ex.Message);
            }
        }

        private static MatriculaDto ToDto(Matricula matricula)
        {
            return new MatriculaDto
            {
                Id = matricula.Id,
                AlunoId = matricula.AlunoId,
                TurmaId = matricula.TurmaId,
                DataMatricula = matricula.DataMatricula
            };
        }
    }
}
