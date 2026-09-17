using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ElPodeFC.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClubConfig",
                columns: table => new
                {
                    Id = table.Column<short>(type: "smallint", nullable: false),
                    DiscordUrl = table.Column<string>(type: "text", nullable: true),
                    HorarioJuego = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    DiasJuego = table.Column<string[]>(type: "text[]", nullable: false),
                    Pausado = table.Column<bool>(type: "boolean", nullable: false),
                    ActualizadoEn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClubConfig", x => x.Id);
                    table.CheckConstraint("CK_club_config_id", "\"Id\" = 1");
                });

            migrationBuilder.CreateTable(
                name: "Formaciones",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Esquema = table.Column<string>(type: "jsonb", nullable: false),
                    CreadoEn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Formaciones", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Jugadores",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Gamertag = table.Column<string>(type: "text", nullable: true),
                    PosicionHabitual = table.Column<string>(type: "text", nullable: true),
                    FotoUrl = table.Column<string>(type: "text", nullable: true),
                    Activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreadoEn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ActualizadoEn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jugadores", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Partidos",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Fecha = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Rival = table.Column<string>(type: "text", nullable: true),
                    Resultado = table.Column<string>(type: "text", nullable: false),
                    MarcadorPropio = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)0),
                    MarcadorRival = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)0),
                    Tipo = table.Column<string>(type: "text", nullable: true),
                    Notas = table.Column<string>(type: "text", nullable: true),
                    RegistradoPor = table.Column<string>(type: "text", nullable: true),
                    CreadoEn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ActualizadoEn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Partidos", x => x.Id);
                    table.CheckConstraint("CK_partidos_resultado", "\"Resultado\" IN ('G', 'E', 'P')");
                });

            migrationBuilder.CreateTable(
                name: "Actuaciones",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PartidoId = table.Column<long>(type: "bigint", nullable: false),
                    JugadorId = table.Column<long>(type: "bigint", nullable: false),
                    Posicion = table.Column<string>(type: "text", nullable: true),
                    Goles = table.Column<short>(type: "smallint", nullable: false),
                    Mvp = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Actuaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Actuaciones_Jugadores_JugadorId",
                        column: x => x.JugadorId,
                        principalTable: "Jugadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Actuaciones_Partidos_PartidoId",
                        column: x => x.PartidoId,
                        principalTable: "Partidos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Medios",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Tipo = table.Column<string>(type: "text", nullable: false, defaultValue: "imagen"),
                    ArchivoRuta = table.Column<string>(type: "text", nullable: true),
                    Url = table.Column<string>(type: "text", nullable: true),
                    PartidoId = table.Column<long>(type: "bigint", nullable: true),
                    Titulo = table.Column<string>(type: "text", nullable: true),
                    SubidoPor = table.Column<string>(type: "text", nullable: true),
                    CreadoEn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Medios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Medios_Partidos_PartidoId",
                        column: x => x.PartidoId,
                        principalTable: "Partidos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.InsertData(
                table: "ClubConfig",
                columns: new[] { "Id", "ActualizadoEn", "DiasJuego", "DiscordUrl", "HorarioJuego", "Pausado" },
                values: new object[] { (short)1, new DateTimeOffset(new DateTime(2026, 9, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new string[0], null, null, false });

            migrationBuilder.CreateIndex(
                name: "IX_Actuaciones_JugadorId",
                table: "Actuaciones",
                column: "JugadorId");

            migrationBuilder.CreateIndex(
                name: "IX_Actuaciones_PartidoId_JugadorId",
                table: "Actuaciones",
                columns: new[] { "PartidoId", "JugadorId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Medios_PartidoId",
                table: "Medios",
                column: "PartidoId");

            migrationBuilder.CreateIndex(
                name: "IX_Partidos_Fecha",
                table: "Partidos",
                column: "Fecha");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Actuaciones");

            migrationBuilder.DropTable(
                name: "ClubConfig");

            migrationBuilder.DropTable(
                name: "Formaciones");

            migrationBuilder.DropTable(
                name: "Medios");

            migrationBuilder.DropTable(
                name: "Jugadores");

            migrationBuilder.DropTable(
                name: "Partidos");
        }
    }
}
