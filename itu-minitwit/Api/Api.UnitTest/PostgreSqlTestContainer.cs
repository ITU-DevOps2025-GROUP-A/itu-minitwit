using Npgsql;
using Respawn;
using Testcontainers.PostgreSql;

namespace Api.UnitTest;

public class PostgreSqlTestContainer : IAsyncLifetime
{
    private readonly PostgreSqlContainer postgresContainer;
    public Respawner Respawner { get; private set; }
    private NpgsqlConnection Connection { get; set; }

    public PostgreSqlTestContainer()
    {
        postgresContainer = new PostgreSqlBuilder()
            .Build();
    }

    public string ConnectionString => postgresContainer.GetConnectionString();

    public async Task InitializeAsync()
    {
        await postgresContainer.StartAsync();
        Connection = new NpgsqlConnection(ConnectionString);
        await Connection.OpenAsync();
    }

    public async Task DisposeAsync()
    {
        await postgresContainer.StopAsync();
        await postgresContainer.DisposeAsync();
    }

    public async Task ResetAsync()
    {
        //reset data
        var respawner = await Respawner.CreateAsync(
            Connection,
            new RespawnerOptions { SchemasToInclude = ["public", "postgres"], DbAdapter = DbAdapter.Postgres }
        );
        await respawner.ResetAsync(Connection);

        //reset id sequences tabelName-coloumName-seq
        using (var command = new NpgsqlCommand("SELECT setval('user_user_id_seq', 1, false);", Connection))
        {
            await command.ExecuteNonQueryAsync();
        }
    }
}