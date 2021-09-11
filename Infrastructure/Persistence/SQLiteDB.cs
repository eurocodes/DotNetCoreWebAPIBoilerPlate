using Core.Application.DTOs.Configurations;
using Infrastructure.Interfaces;
using Microsoft.Extensions.Options;
using NetCore.AutoRegisterDi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Database {

    [DoNotAutoRegister]
    public class SQLiteDB : IDBCommand, IDisposable {
        private DBConfig _parameters;
        private SQLiteConnection connection;
        private SQLiteCommand command;
        private SQLiteTransaction transaction;
        private string statement;
        public int lastAffectedRows { get; set; }
        public SQLiteDB(IOptionsMonitor<SystemVariables> config) {
            _parameters = config.CurrentValue.SQLite;
            _connect();
        }

        private void _connect() {
            this.connection = new SQLiteConnection(string.Concat("data source=", _parameters.database));
        }

        public void beginTransaction() {
            _generateCommand();            
            transaction = connection.BeginTransaction();
            command.Transaction = transaction;
        }

        public void prepare(string sql, bool isAt = true) {
            sql = isAt ? sql.Replace(':', '@') : sql;
            string[] strings = sql.Split('?');
            string nstr = "";
            if (strings.Length == 1) {
                nstr = sql;
            } else {
                for (int i = 0; i < strings.Length; i++) {
                    if (!string.IsNullOrWhiteSpace(strings[i])) {
                        string trimmed = strings[i].TrimEnd();
                        if (i + 1 != strings.Length) {
                            nstr += strings[i] + " @bind" + i.ToString();
                        } else {
                            nstr += strings[i];
                        }
                    }
                }
            }
            statement = nstr;
            try {
                if (command == null)
                    _generateCommand();
                command.CommandText = statement;
                command.CommandType = System.Data.CommandType.Text;
                clearParams();
            } catch (Exception) {
                this.command = null;
            }
        }

        public void bindValue(string key, object value) {
            command.Parameters.AddWithValue(key, value);
        }

        public void bindValues(IEnumerable<object> valueObj) {
            clearParams();
            List<object> values = (List<object>)valueObj;
            for (int i = 0; i < values.Count; i++) {
                command.Parameters.AddWithValue("@bind" + i, values[i]);
            }
        }

        public bool clearParams() {
            command.Parameters.Clear();
            return true;
        }

        public async Task<bool> execute() {
            try {
                if (connection != null && connection.State == ConnectionState.Closed)
                    await connection.OpenAsync();
                lastAffectedRows = await command.ExecuteNonQueryAsync();
                return true;
            } catch (Exception e) {
                try {
                    await transaction.RollbackAsync();
                } catch { }
                throw e;
            }
        }
        public async Task<IEnumerable<Dictionary<string, object>>> fetchAllAsObj() {
            DataTable data = new DataTable();
            var reader = await command.ExecuteReaderAsync();
            data.Load(reader);
            reader.Close();
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            Dictionary<string, object> row;
            foreach (DataRow dr in data.Rows) {
                row = new Dictionary<string, object>();
                foreach (DataColumn col in data.Columns) {
                    row.Add(col.ColumnName, dr[col]);
                }
                rows.Add(row);
            }
            return rows;
        }

        public async Task<string> fetchAllAsStr() {
            List<Dictionary<string, object>> res =(List<Dictionary<string, object>>) await fetchAllAsObj();
            res = res ?? new List<Dictionary<string, object>>();
            return JsonConvert.SerializeObject(res, Formatting.Indented);
        }

        public async Task<bool> commit() {
            try {
                await transaction.CommitAsync();
            } catch (Exception err) {
                await transaction.RollbackAsync();
                throw err;
            }
            command = null;
            return true;
        }

        private void _generateCommand() {
            SQLiteCommand command;
            try {
                command = connection.CreateCommand();
                command.Connection = this.connection;
            } catch (Exception) {
                command = null;
            }
            this.command = command;
        }

        public void Dispose() {
            close();
        }
        public void close() {
            try {
                connection.CloseAsync();
                connection.Dispose();
            } catch { }
        }
    }
}
