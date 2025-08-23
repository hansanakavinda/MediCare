using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MediCare.Migrations
{
    /// <inheritdoc />
    public partial class FixFeedbackRatingType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "APPUSER");

            migrationBuilder.CreateTable(
                name: "CLINIC",
                schema: "APPUSER",
                columns: table => new
                {
                    CLINIC_ID = table.Column<decimal>(type: "NUMBER", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NAME = table.Column<string>(type: "VARCHAR2(200)", unicode: false, maxLength: 200, nullable: false),
                    ADDRESS = table.Column<string>(type: "VARCHAR2(400)", unicode: false, maxLength: 400, nullable: true),
                    PHONE = table.Column<string>(type: "VARCHAR2(40)", unicode: false, maxLength: 40, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("SYS_C0013568", x => x.CLINIC_ID);
                });

            migrationBuilder.CreateTable(
                name: "SPECIALTY",
                schema: "APPUSER",
                columns: table => new
                {
                    SPECIALTY_ID = table.Column<decimal>(type: "NUMBER", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NAME = table.Column<string>(type: "VARCHAR2(120)", unicode: false, maxLength: 120, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("SYS_C0013564", x => x.SPECIALTY_ID);
                });

            migrationBuilder.CreateTable(
                name: "USERS",
                schema: "APPUSER",
                columns: table => new
                {
                    USER_ID = table.Column<decimal>(type: "NUMBER", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    EMAIL = table.Column<string>(type: "VARCHAR2(255)", unicode: false, maxLength: 255, nullable: false),
                    PWD = table.Column<string>(type: "VARCHAR2(255)", unicode: false, maxLength: 255, nullable: false),
                    FULL_NAME = table.Column<string>(type: "VARCHAR2(200)", unicode: false, maxLength: 200, nullable: false),
                    PHONE = table.Column<string>(type: "VARCHAR2(40)", unicode: false, maxLength: 40, nullable: true),
                    ROLE = table.Column<string>(type: "VARCHAR2(20)", unicode: false, maxLength: 20, nullable: false),
                    IS_ACTIVE = table.Column<string>(type: "CHAR(1)", unicode: false, fixedLength: true, maxLength: 1, nullable: false, defaultValueSql: "'Y' "),
                    CREATED_AT = table.Column<DateTime>(type: "TIMESTAMP(6)", precision: 6, nullable: false, defaultValueSql: "SYSTIMESTAMP ")
                },
                constraints: table =>
                {
                    table.PrimaryKey("SYS_C0013560", x => x.USER_ID);
                });

            migrationBuilder.CreateTable(
                name: "DOCTOR",
                schema: "APPUSER",
                columns: table => new
                {
                    DOCTOR_ID = table.Column<decimal>(type: "NUMBER", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    USER_ID = table.Column<decimal>(type: "NUMBER", nullable: false),
                    SPECIALTY_ID = table.Column<decimal>(type: "NUMBER", nullable: true),
                    CLINIC_ID = table.Column<decimal>(type: "NUMBER", nullable: true),
                    BIO = table.Column<string>(type: "CLOB", nullable: true),
                    FEE = table.Column<decimal>(type: "NUMBER(10,2)", nullable: true, defaultValueSql: "0")
                },
                constraints: table =>
                {
                    table.PrimaryKey("SYS_C0013571", x => x.DOCTOR_ID);
                    table.ForeignKey(
                        name: "FK_DOCTOR_CLINIC",
                        column: x => x.CLINIC_ID,
                        principalSchema: "APPUSER",
                        principalTable: "CLINIC",
                        principalColumn: "CLINIC_ID");
                    table.ForeignKey(
                        name: "FK_DOCTOR_SPECIALTY",
                        column: x => x.SPECIALTY_ID,
                        principalSchema: "APPUSER",
                        principalTable: "SPECIALTY",
                        principalColumn: "SPECIALTY_ID");
                    table.ForeignKey(
                        name: "FK_DOCTOR_USER",
                        column: x => x.USER_ID,
                        principalSchema: "APPUSER",
                        principalTable: "USERS",
                        principalColumn: "USER_ID");
                });

            migrationBuilder.CreateTable(
                name: "PATIENT",
                schema: "APPUSER",
                columns: table => new
                {
                    PATIENT_ID = table.Column<decimal>(type: "NUMBER", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    USER_ID = table.Column<decimal>(type: "NUMBER", nullable: false),
                    DOB = table.Column<DateTime>(type: "DATE", nullable: true),
                    GENDER = table.Column<string>(type: "VARCHAR2(20)", unicode: false, maxLength: 20, nullable: true),
                    ADDRESS = table.Column<string>(type: "VARCHAR2(400)", unicode: false, maxLength: 400, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("SYS_C0013578", x => x.PATIENT_ID);
                    table.ForeignKey(
                        name: "FK_PATIENT_USER",
                        column: x => x.USER_ID,
                        principalSchema: "APPUSER",
                        principalTable: "USERS",
                        principalColumn: "USER_ID");
                });

            migrationBuilder.CreateTable(
                name: "SCHEDULE",
                schema: "APPUSER",
                columns: table => new
                {
                    SCHEDULE_ID = table.Column<decimal>(type: "NUMBER", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    DOCTOR_ID = table.Column<decimal>(type: "NUMBER", nullable: false),
                    DAY_OF_WEEK = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    START_TIME = table.Column<string>(type: "VARCHAR2(5)", unicode: false, maxLength: 5, nullable: false),
                    END_TIME = table.Column<string>(type: "VARCHAR2(5)", unicode: false, maxLength: 5, nullable: false),
                    SLOT_MINUTES = table.Column<byte>(type: "NUMBER(3)", precision: 3, nullable: true, defaultValueSql: "30")
                },
                constraints: table =>
                {
                    table.PrimaryKey("SYS_C0013587", x => x.SCHEDULE_ID);
                    table.ForeignKey(
                        name: "FK_SCHEDULE_DOCTOR",
                        column: x => x.DOCTOR_ID,
                        principalSchema: "APPUSER",
                        principalTable: "DOCTOR",
                        principalColumn: "DOCTOR_ID");
                });

            migrationBuilder.CreateTable(
                name: "FEEDBACK",
                schema: "APPUSER",
                columns: table => new
                {
                    FEEDBACK_ID = table.Column<decimal>(type: "NUMBER", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    PATIENT_ID = table.Column<decimal>(type: "NUMBER", nullable: false),
                    DOCTOR_ID = table.Column<decimal>(type: "NUMBER", nullable: false),
                    RATING = table.Column<decimal>(type: "NUMBER", nullable: false),
                    MSG = table.Column<string>(type: "CLOB", nullable: true),
                    CREATED_AT = table.Column<DateTime>(type: "TIMESTAMP(6)", precision: 6, nullable: false, defaultValueSql: "SYSTIMESTAMP ")
                },
                constraints: table =>
                {
                    table.PrimaryKey("SYS_C0013618", x => x.FEEDBACK_ID);
                    table.ForeignKey(
                        name: "FK_FEEDBACK_DOCTOR",
                        column: x => x.DOCTOR_ID,
                        principalSchema: "APPUSER",
                        principalTable: "DOCTOR",
                        principalColumn: "DOCTOR_ID");
                    table.ForeignKey(
                        name: "FK_FEEDBACK_PATIENT",
                        column: x => x.PATIENT_ID,
                        principalSchema: "APPUSER",
                        principalTable: "PATIENT",
                        principalColumn: "PATIENT_ID");
                });

            migrationBuilder.CreateTable(
                name: "APPOINTMENT",
                schema: "APPUSER",
                columns: table => new
                {
                    APPOINTMENT_ID = table.Column<decimal>(type: "NUMBER", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    PATIENT_ID = table.Column<decimal>(type: "NUMBER", nullable: false),
                    DOCTOR_ID = table.Column<decimal>(type: "NUMBER", nullable: false),
                    SCHEDULE_ID = table.Column<decimal>(type: "NUMBER", nullable: true),
                    SCHEDULED_AT = table.Column<DateTime>(type: "TIMESTAMP(6)", precision: 6, nullable: false),
                    STATUS = table.Column<string>(type: "VARCHAR2(20)", unicode: false, maxLength: 20, nullable: false, defaultValueSql: "'Pending' "),
                    CREATED_AT = table.Column<DateTime>(type: "TIMESTAMP(6)", precision: 6, nullable: false, defaultValueSql: "SYSTIMESTAMP "),
                    UPDATED_AT = table.Column<DateTime>(type: "TIMESTAMP(6)", precision: 6, nullable: true),
                    NOTES = table.Column<string>(type: "CLOB", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("SYS_C0013596", x => x.APPOINTMENT_ID);
                    table.ForeignKey(
                        name: "FK_APPOINTMENT_DOCTOR",
                        column: x => x.DOCTOR_ID,
                        principalSchema: "APPUSER",
                        principalTable: "DOCTOR",
                        principalColumn: "DOCTOR_ID");
                    table.ForeignKey(
                        name: "FK_APPOINTMENT_PATIENT",
                        column: x => x.PATIENT_ID,
                        principalSchema: "APPUSER",
                        principalTable: "PATIENT",
                        principalColumn: "PATIENT_ID");
                    table.ForeignKey(
                        name: "FK_APPOINTMENT_SCHEDULE",
                        column: x => x.SCHEDULE_ID,
                        principalSchema: "APPUSER",
                        principalTable: "SCHEDULE",
                        principalColumn: "SCHEDULE_ID");
                });

            migrationBuilder.CreateTable(
                name: "PAYMENT",
                schema: "APPUSER",
                columns: table => new
                {
                    PAYMENT_ID = table.Column<decimal>(type: "NUMBER", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    APPOINTMENT_ID = table.Column<decimal>(type: "NUMBER", nullable: false),
                    AMOUNT = table.Column<decimal>(type: "NUMBER(12,2)", nullable: false),
                    METHOD = table.Column<string>(type: "VARCHAR2(20)", unicode: false, maxLength: 20, nullable: true, defaultValueSql: "'credit card'"),
                    STATUS = table.Column<string>(type: "VARCHAR2(20)", unicode: false, maxLength: 20, nullable: true, defaultValueSql: "'Pending'"),
                    PAID_AT = table.Column<DateTime>(type: "TIMESTAMP(6)", precision: 6, nullable: true),
                    TXN_REF = table.Column<string>(type: "VARCHAR2(200)", unicode: false, maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("SYS_C0013605", x => x.PAYMENT_ID);
                    table.ForeignKey(
                        name: "FK_PAYMENT_APPOINTMENT",
                        column: x => x.APPOINTMENT_ID,
                        principalSchema: "APPUSER",
                        principalTable: "APPOINTMENT",
                        principalColumn: "APPOINTMENT_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PRESCRIPTION",
                schema: "APPUSER",
                columns: table => new
                {
                    PRESCRIPTION_ID = table.Column<decimal>(type: "NUMBER", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    APPOINTMENT_ID = table.Column<decimal>(type: "NUMBER", nullable: false),
                    CONTENT = table.Column<string>(type: "CLOB", nullable: true),
                    CREATED_AT = table.Column<DateTime>(type: "TIMESTAMP(6)", precision: 6, nullable: false, defaultValueSql: "SYSTIMESTAMP ")
                },
                constraints: table =>
                {
                    table.PrimaryKey("SYS_C0013610", x => x.PRESCRIPTION_ID);
                    table.ForeignKey(
                        name: "FK_PRESCRIPTION_APPOINTMENT",
                        column: x => x.APPOINTMENT_ID,
                        principalSchema: "APPUSER",
                        principalTable: "APPOINTMENT",
                        principalColumn: "APPOINTMENT_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IDX_APPOINTMENT_DOCTOR",
                schema: "APPUSER",
                table: "APPOINTMENT",
                column: "DOCTOR_ID");

            migrationBuilder.CreateIndex(
                name: "IDX_APPOINTMENT_PATIENT",
                schema: "APPUSER",
                table: "APPOINTMENT",
                column: "PATIENT_ID");

            migrationBuilder.CreateIndex(
                name: "IX_APPOINTMENT_SCHEDULE_ID",
                schema: "APPUSER",
                table: "APPOINTMENT",
                column: "SCHEDULE_ID");

            migrationBuilder.CreateIndex(
                name: "IDX_DOCTOR_CLINIC",
                schema: "APPUSER",
                table: "DOCTOR",
                column: "CLINIC_ID");

            migrationBuilder.CreateIndex(
                name: "IDX_DOCTOR_SPECIALTY",
                schema: "APPUSER",
                table: "DOCTOR",
                column: "SPECIALTY_ID");

            migrationBuilder.CreateIndex(
                name: "UQ_DOCTOR_USER",
                schema: "APPUSER",
                table: "DOCTOR",
                column: "USER_ID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FEEDBACK_DOCTOR_ID",
                schema: "APPUSER",
                table: "FEEDBACK",
                column: "DOCTOR_ID");

            migrationBuilder.CreateIndex(
                name: "IX_FEEDBACK_PATIENT_ID",
                schema: "APPUSER",
                table: "FEEDBACK",
                column: "PATIENT_ID");

            migrationBuilder.CreateIndex(
                name: "UQ_PATIENT_USER",
                schema: "APPUSER",
                table: "PATIENT",
                column: "USER_ID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IDX_PAYMENT_APPOINTMENT",
                schema: "APPUSER",
                table: "PAYMENT",
                column: "APPOINTMENT_ID");

            migrationBuilder.CreateIndex(
                name: "IX_PRESCRIPTION_APPOINTMENT_ID",
                schema: "APPUSER",
                table: "PRESCRIPTION",
                column: "APPOINTMENT_ID");

            migrationBuilder.CreateIndex(
                name: "IDX_SCHEDULE_DOCTOR",
                schema: "APPUSER",
                table: "SCHEDULE",
                column: "DOCTOR_ID");

            migrationBuilder.CreateIndex(
                name: "UQ_SPECIALTY_NAME",
                schema: "APPUSER",
                table: "SPECIALTY",
                column: "NAME",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_USERS_EMAIL",
                schema: "APPUSER",
                table: "USERS",
                column: "EMAIL",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FEEDBACK",
                schema: "APPUSER");

            migrationBuilder.DropTable(
                name: "PAYMENT",
                schema: "APPUSER");

            migrationBuilder.DropTable(
                name: "PRESCRIPTION",
                schema: "APPUSER");

            migrationBuilder.DropTable(
                name: "APPOINTMENT",
                schema: "APPUSER");

            migrationBuilder.DropTable(
                name: "PATIENT",
                schema: "APPUSER");

            migrationBuilder.DropTable(
                name: "SCHEDULE",
                schema: "APPUSER");

            migrationBuilder.DropTable(
                name: "DOCTOR",
                schema: "APPUSER");

            migrationBuilder.DropTable(
                name: "CLINIC",
                schema: "APPUSER");

            migrationBuilder.DropTable(
                name: "SPECIALTY",
                schema: "APPUSER");

            migrationBuilder.DropTable(
                name: "USERS",
                schema: "APPUSER");
        }
    }
}
