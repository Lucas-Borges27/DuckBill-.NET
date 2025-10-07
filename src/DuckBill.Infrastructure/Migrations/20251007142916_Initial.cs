using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DuckBill.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ATIVO",
                columns: table => new
                {
                    ID = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    TICKER = table.Column<string>(type: "NVARCHAR2(12)", maxLength: 12, nullable: false),
                    TIPO = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    MOEDA_BASE = table.Column<string>(type: "NVARCHAR2(3)", maxLength: 3, nullable: false, defaultValue: "BRL")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ATIVO", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "CATEGORIA",
                columns: table => new
                {
                    ID = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NOME = table.Column<string>(type: "NVARCHAR2(60)", maxLength: 60, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CATEGORIA", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "USUARIO",
                columns: table => new
                {
                    ID = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NOME = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    EMAIL = table.Column<string>(type: "NVARCHAR2(120)", maxLength: 120, nullable: false),
                    SENHA = table.Column<string>(type: "NVARCHAR2(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USUARIO", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "COTACAO_ATIVO",
                columns: table => new
                {
                    ATIVO_ID = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    DATA_REF = table.Column<DateTime>(type: "DATE", nullable: false),
                    PRECO_FECH = table.Column<decimal>(type: "NUMBER(14,6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_COTACAO_ATIVO", x => new { x.ATIVO_ID, x.DATA_REF });
                    table.ForeignKey(
                        name: "FK_COTACAO_ATIVO_ATIVO_ATIVO_ID",
                        column: x => x.ATIVO_ID,
                        principalTable: "ATIVO",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DESPESA",
                columns: table => new
                {
                    ID = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    USUARIO_ID = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    CATEGORIA_ID = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    VALOR = table.Column<decimal>(type: "NUMBER(12,2)", nullable: false),
                    MOEDA = table.Column<string>(type: "NVARCHAR2(3)", maxLength: 3, nullable: false, defaultValue: "BRL"),
                    DATA_COMPRA = table.Column<DateTime>(type: "DATE", nullable: false),
                    DESCRICAO = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DESPESA", x => x.ID);
                    table.ForeignKey(
                        name: "FK_DESPESA_CATEGORIA_CATEGORIA_ID",
                        column: x => x.CATEGORIA_ID,
                        principalTable: "CATEGORIA",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DESPESA_USUARIO_USUARIO_ID",
                        column: x => x.USUARIO_ID,
                        principalTable: "USUARIO",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TRANSACAO_ATIVO",
                columns: table => new
                {
                    ID = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    USUARIO_ID = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    ATIVO_ID = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    TIPO = table.Column<string>(type: "NVARCHAR2(4)", maxLength: 4, nullable: false),
                    QTD = table.Column<decimal>(type: "NUMBER(12,4)", nullable: false),
                    PRECO = table.Column<decimal>(type: "NUMBER(14,6)", nullable: false),
                    DATA_NEGOCIO = table.Column<DateTime>(type: "DATE", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TRANSACAO_ATIVO", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TRANSACAO_ATIVO_ATIVO_ATIVO_ID",
                        column: x => x.ATIVO_ID,
                        principalTable: "ATIVO",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TRANSACAO_ATIVO_USUARIO_USUARIO_ID",
                        column: x => x.USUARIO_ID,
                        principalTable: "USUARIO",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ATIVO_TICKER",
                table: "ATIVO",
                column: "TICKER",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CATEGORIA_NOME",
                table: "CATEGORIA",
                column: "NOME",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DESPESA_CATEGORIA_ID",
                table: "DESPESA",
                column: "CATEGORIA_ID");

            migrationBuilder.CreateIndex(
                name: "IX_DESPESA_USUARIO_ID",
                table: "DESPESA",
                column: "USUARIO_ID");

            migrationBuilder.CreateIndex(
                name: "IX_TRANSACAO_ATIVO_ATIVO_ID",
                table: "TRANSACAO_ATIVO",
                column: "ATIVO_ID");

            migrationBuilder.CreateIndex(
                name: "IX_TRANSACAO_ATIVO_USUARIO_ID",
                table: "TRANSACAO_ATIVO",
                column: "USUARIO_ID");

            migrationBuilder.CreateIndex(
                name: "IX_USUARIO_EMAIL",
                table: "USUARIO",
                column: "EMAIL",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "COTACAO_ATIVO");

            migrationBuilder.DropTable(
                name: "DESPESA");

            migrationBuilder.DropTable(
                name: "TRANSACAO_ATIVO");

            migrationBuilder.DropTable(
                name: "CATEGORIA");

            migrationBuilder.DropTable(
                name: "ATIVO");

            migrationBuilder.DropTable(
                name: "USUARIO");
        }
    }
}
