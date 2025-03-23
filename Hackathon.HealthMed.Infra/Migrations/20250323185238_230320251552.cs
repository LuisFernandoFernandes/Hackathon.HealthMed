using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hackathon.HealthMed.Infra.Migrations
{
    /// <inheritdoc />
    public partial class _230320251552 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Horarios_MedicoId",
                table: "Horarios");

            migrationBuilder.DropIndex(
                name: "IX_Agendamentos_HorarioId",
                table: "Agendamentos");

            migrationBuilder.CreateIndex(
                name: "IX_Horarios_Medico_DataHorario",
                table: "Horarios",
                columns: new[] { "MedicoId", "DataHorario" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Agendamentos_HorarioId",
                table: "Agendamentos",
                column: "HorarioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Horarios_Medico_DataHorario",
                table: "Horarios");

            migrationBuilder.DropIndex(
                name: "IX_Agendamentos_HorarioId",
                table: "Agendamentos");

            migrationBuilder.CreateIndex(
                name: "IX_Horarios_MedicoId",
                table: "Horarios",
                column: "MedicoId");

            migrationBuilder.CreateIndex(
                name: "IX_Agendamentos_HorarioId",
                table: "Agendamentos",
                column: "HorarioId",
                unique: true);
        }
    }
}
