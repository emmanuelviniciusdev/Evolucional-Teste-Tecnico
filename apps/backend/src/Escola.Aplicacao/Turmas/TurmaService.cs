using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Escola.Dominio;
using Escola.Dominio.Entidades;
using Escola.Dominio.Repositorios;

namespace Escola.Aplicacao.Turmas
{
    public class TurmaService
    {
        private readonly ITurmaRepository _turmaRepository;
        private readonly ICacheService _cacheService;

        public TurmaService(ITurmaRepository turmaRepository, ICacheService cacheService)
        {
            _turmaRepository = turmaRepository;
            _cacheService = cacheService;
        }

        public async Task<IReadOnlyList<TurmaDto>> ListarAsync()
        {
            try
            {
                var cached = await _cacheService.GetAsync<List<TurmaDto>>(CacheKeys.TurmasListagem).ConfigureAwait(false);
                if (cached != null)
                {
                    return cached;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceWarning("Failed to read turma listing cache: {0}", ex.Message);
            }

            var turmas = await _turmaRepository.ListAsync().ConfigureAwait(false);
            var dtos = turmas.Select(ToDto).ToList();

            try
            {
                await _cacheService.SetAsync(CacheKeys.TurmasListagem, dtos, CacheKeys.TurmasListagemTtl)
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Trace.TraceWarning("Failed to write turma listing cache: {0}", ex.Message);
            }

            return dtos;
        }

        private static TurmaDto ToDto(Turma turma)
        {
            return new TurmaDto
            {
                Id = turma.Id,
                Nome = turma.Nome,
                Periodo = turma.Periodo,
                VagasTotal = turma.VagasTotal,
                VagasDisponiveis = turma.VagasDisponiveis
            };
        }
    }
}
