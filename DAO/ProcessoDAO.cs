
using AppWebExemplo.Configs;
using AppWebExemplo.Model;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace AppWebExemplo.DAO
{
    public class ProcessoDAO
    {
        private readonly Conexao _conexao;

        public ProcessoDAO(Conexao conexao)
        {
            _conexao = conexao;
        }

        // Lista todos os processos cadastrados.
        public List<Processo> Listar()
        {
            var lista = new List<Processo>();

            using var con = _conexao.GetConnection();

            const string sql = @"
                SELECT id_pro, numero_pro, data_pro,
                       interessado_pro, assunto_pro,
                       descricao_pro, situacao_pro
                FROM processos";

            using var comando = con.CreateCommand();
            comando.CommandText = sql;

            using var leitor = comando.ExecuteReader();

            while (leitor.Read())
            {
                var processo = new Processo
                {
                    Id = leitor.GetInt32("id_pro"),
                    Numero = leitor.GetString("numero_pro"),

                    Data = DAOHelper.GetDateTime(
                        leitor, "data_pro") is DateTime data
                            ? DateOnly.FromDateTime(data)
                            : null,

                    Interessado = leitor.GetString("interessado_pro"),
                    Assunto = leitor.GetString("assunto_pro"),
                    Descricao = DAOHelper.GetString(
                        leitor, "descricao_pro"),
                    Situacao = leitor.GetString("situacao_pro")
                };

                lista.Add(processo);
            }

            return lista;
        }

        // Busca um processo pelo ID.
        // Retorna null quando o processo não existe.
        public Processo? ObterPorId(int id)
        {
            using var con = _conexao.GetConnection();

            const string sql = @"
                SELECT id_pro, numero_pro, data_pro,
                       interessado_pro, assunto_pro,
                       descricao_pro, situacao_pro
                FROM processos
                WHERE id_pro = @id";

            using var comando = con.CreateCommand();
            comando.CommandText = sql;
            comando.Parameters.AddWithValue("@id", id);

            using var leitor = comando.ExecuteReader();

            if (!leitor.Read())
            {
                return null;
            }

            return new Processo
            {
                Id = leitor.GetInt32("id_pro"),
                Numero = leitor.GetString("numero_pro"),

                Data = DAOHelper.GetDateTime(
                    leitor, "data_pro") is DateTime data
                        ? DateOnly.FromDateTime(data)
                        : null,

                Interessado = leitor.GetString("interessado_pro"),
                Assunto = leitor.GetString("assunto_pro"),
                Descricao = DAOHelper.GetString(
                    leitor, "descricao_pro"),
                Situacao = leitor.GetString("situacao_pro")
            };
        }

        // Insere um novo processo.
        public void Inserir(Processo processo)
        {
            using var con = _conexao.GetConnection();

            const string sql = @"
                INSERT INTO processos
                    (numero_pro, data_pro, interessado_pro,
                     assunto_pro, descricao_pro, situacao_pro)
                VALUES
                    (@numero, @data, @interessado,
                     @assunto, @descricao, @situacao)";

            using var comando = con.CreateCommand();
            comando.CommandText = sql;

            comando.Parameters.AddWithValue(
                "@numero", processo.Numero);

            comando.Parameters.AddWithValue(
                "@data",
                processo.Data.HasValue
                    ? processo.Data.Value.ToDateTime(TimeOnly.MinValue)
                    : DBNull.Value);

            comando.Parameters.AddWithValue(
                "@interessado", processo.Interessado);

            comando.Parameters.AddWithValue(
                "@assunto", processo.Assunto);

            comando.Parameters.AddWithValue(
                "@descricao",
                string.IsNullOrEmpty(processo.Descricao)
                    ? DBNull.Value
                    : processo.Descricao);

            comando.Parameters.AddWithValue(
                "@situacao", processo.Situacao);

            comando.ExecuteNonQuery();
        }

        // Atualiza um processo existente.
        public void Atualizar(Processo processo)
        {
            using var con = _conexao.GetConnection();

            const string sql = @"
                UPDATE processos
                SET numero_pro = @numero,
                    data_pro = @data,
                    interessado_pro = @interessado,
                    assunto_pro = @assunto,
                    descricao_pro = @descricao,
                    situacao_pro = @situacao
                WHERE id_pro = @id";

            using var comando = con.CreateCommand();
            comando.CommandText = sql;

            comando.Parameters.AddWithValue(
                "@numero", processo.Numero);

            comando.Parameters.AddWithValue(
                "@data",
                processo.Data.HasValue
                    ? processo.Data.Value.ToDateTime(TimeOnly.MinValue)
                    : DBNull.Value);

            comando.Parameters.AddWithValue(
                "@interessado", processo.Interessado);

            comando.Parameters.AddWithValue(
                "@assunto", processo.Assunto);

            comando.Parameters.AddWithValue(
                "@descricao",
                string.IsNullOrEmpty(processo.Descricao)
                    ? DBNull.Value
                    : processo.Descricao);

            comando.Parameters.AddWithValue(
                "@situacao", processo.Situacao);

            comando.Parameters.AddWithValue(
                "@id", processo.Id);

            comando.ExecuteNonQuery();
        }

        // Exclui permanentemente um processo pelo ID.
        public void Excluir(int id)
        {
            using var con = _conexao.GetConnection();

            const string sql = @"
                DELETE FROM processos
                WHERE id_pro = @id";

            using var comando = con.CreateCommand();
            comando.CommandText = sql;
            comando.Parameters.AddWithValue("@id", id);

            comando.ExecuteNonQuery();
        }
    }
}
