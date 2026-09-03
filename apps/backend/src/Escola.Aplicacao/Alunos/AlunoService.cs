using System;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Escola.Dominio.Entidades;
using Escola.Dominio.Excecoes;
using Escola.Dominio.Repositorios;

namespace Escola.Aplicacao.Alunos
{
    public class AlunoService
    {
        private const int NomeEmailMaxLength = 120;
        private const int PaginaPadrao = 1;
        private const int TamanhoPaginaPadrao = 10;
        private const int TamanhoPaginaMaximo = 100;

        private readonly IAlunoRepository _alunoRepository;

        public AlunoService(IAlunoRepository alunoRepository)
        {
            _alunoRepository = alunoRepository;
        }

        public async Task<ListaAlunosDto> ListarAsync(string nome, int? pagina, int? tamanhoPagina)
        {
            var paginaEfetiva = pagina ?? PaginaPadrao;
            var tamanhoEfetivo = tamanhoPagina ?? TamanhoPaginaPadrao;

            if (paginaEfetiva < 1)
            {
                throw new ValidationException("pagina deve ser maior ou igual a 1.");
            }

            if (tamanhoEfetivo < 1 || tamanhoEfetivo > TamanhoPaginaMaximo)
            {
                throw new ValidationException("tamanhoPagina deve estar entre 1 e 100.");
            }

            var filtro = string.IsNullOrWhiteSpace(nome) ? null : nome.Trim();
            var offset = (paginaEfetiva - 1) * tamanhoEfetivo;
            var alunos = await _alunoRepository.ListAsync(filtro, offset, tamanhoEfetivo).ConfigureAwait(false);
            var total = await _alunoRepository.CountAsync(filtro).ConfigureAwait(false);

            return new ListaAlunosDto
            {
                Alunos = alunos.Select(ToDto).ToList(),
                Total = total,
                Pagina = paginaEfetiva,
                TamanhoPagina = tamanhoEfetivo
            };
        }

        public async Task<AlunoDto> ObterPorIdAsync(int id)
        {
            var aluno = await _alunoRepository.GetByIdAsync(id).ConfigureAwait(false);
            if (aluno == null || !aluno.Ativo)
            {
                throw new NotFoundException("Aluno não encontrado.");
            }

            return ToDto(aluno);
        }

        public async Task<AlunoDto> CriarAsync(AlunoEscritaDto dto)
        {
            var aluno = ValidarEscrita(dto);
            aluno.Ativo = true;
            var id = await _alunoRepository.InsertAsync(aluno).ConfigureAwait(false);
            return await ObterPorIdAsync(id).ConfigureAwait(false);
        }

        public async Task<AlunoDto> AtualizarAsync(int id, AlunoEscritaDto dto)
        {
            var existente = await _alunoRepository.GetByIdAsync(id).ConfigureAwait(false);
            if (existente == null)
            {
                throw new NotFoundException("Aluno não encontrado.");
            }

            var atualizado = ValidarEscrita(dto);
            atualizado.Id = id;
            await _alunoRepository.UpdateAsync(atualizado).ConfigureAwait(false);
            var persistido = await _alunoRepository.GetByIdAsync(id).ConfigureAwait(false);
            return ToDto(persistido);
        }

        public async Task ExcluirLogicamenteAsync(int id)
        {
            var existente = await _alunoRepository.GetByIdAsync(id).ConfigureAwait(false);
            if (existente == null)
            {
                throw new NotFoundException("Aluno não encontrado.");
            }

            await _alunoRepository.LogicalDeleteAsync(id).ConfigureAwait(false);
        }

        private static Aluno ValidarEscrita(AlunoEscritaDto dto)
        {
            if (dto == null)
            {
                throw new ValidationException("Dados do aluno são obrigatórios.");
            }

            var nome = dto.Nome == null ? null : dto.Nome.Trim();
            if (string.IsNullOrEmpty(nome))
            {
                throw new ValidationException("Nome é obrigatório.");
            }

            if (nome.Length > NomeEmailMaxLength)
            {
                throw new ValidationException("Nome deve ter no máximo 120 caracteres.");
            }

            ValidarEmail(dto.Email);

            return new Aluno
            {
                Nome = nome,
                Email = dto.Email,
                DataNascimento = ParseDataNascimento(dto.DataNascimento)
            };
        }

        private static void ValidarEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ValidationException("E-mail é obrigatório.");
            }

            if (email.Length > NomeEmailMaxLength)
            {
                throw new ValidationException("E-mail deve ter no máximo 120 caracteres.");
            }

            if (email.IndexOf(' ') >= 0 || email != email.Trim())
            {
                throw new ValidationException("E-mail inválido.");
            }

            MailAddress address;
            try
            {
                address = new MailAddress(email);
            }
            catch (FormatException)
            {
                throw new ValidationException("E-mail inválido.");
            }

            if (!string.Equals(address.Address, email, StringComparison.Ordinal))
            {
                throw new ValidationException("E-mail inválido.");
            }

            var labels = address.Host.Split('.');
            if (labels.Length < 2 || labels.Any(string.IsNullOrWhiteSpace))
            {
                throw new ValidationException("E-mail inválido.");
            }
        }

        private static DateTime ParseDataNascimento(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ValidationException("Data de nascimento é obrigatória.");
            }

            DateTime date;
            if (!DateTime.TryParseExact(
                    value,
                    "yyyy-MM-dd",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out date))
            {
                throw new ValidationException("Data de nascimento deve estar no formato YYYY-MM-DD.");
            }

            if (date.Date > DateTime.Today)
            {
                throw new ValidationException("Data de nascimento não pode ser uma data futura.");
            }

            return date;
        }

        private static AlunoDto ToDto(Aluno aluno)
        {
            return new AlunoDto
            {
                Id = aluno.Id,
                Nome = aluno.Nome,
                Email = aluno.Email,
                DataNascimento = aluno.DataNascimento,
                Ativo = aluno.Ativo,
                DataCadastro = aluno.DataCadastro
            };
        }
    }
}
