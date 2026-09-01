using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExamSystem.Migrations
{
    /// <inheritdoc />
    public partial class InitialExamSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "m_admin_users",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    username = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    email = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    password_hash = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    full_name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    phone = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    profile_image = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    role = table.Column<short>(type: "smallint", nullable: false),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    password_reset_token = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    password_reset_expiry = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    created_user_id = table.Column<long>(type: "bigint", nullable: true),
                    updated_user_id = table.Column<long>(type: "bigint", nullable: true),
                    created_datetime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_datetime = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_m_admin_users", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "m_grades",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    level = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    description = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    sort_order = table.Column<int>(type: "int", nullable: false),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    created_user_id = table.Column<long>(type: "bigint", nullable: true),
                    updated_user_id = table.Column<long>(type: "bigint", nullable: true),
                    created_datetime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_datetime = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_m_grades", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "m_tokens",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    session_token = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    jwt_token = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    expires_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    is_revoked = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ip_address = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    user_agent = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_m_tokens", x => x.id);
                    table.ForeignKey(
                        name: "FK_m_tokens_m_admin_users_user_id",
                        column: x => x.user_id,
                        principalTable: "m_admin_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "m_subjects",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    grade_id = table.Column<long>(type: "bigint", nullable: false),
                    name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    code = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    description = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    total_marks = table.Column<int>(type: "int", nullable: false),
                    pass_marks = table.Column<int>(type: "int", nullable: false),
                    duration_minutes = table.Column<int>(type: "int", nullable: false),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    created_user_id = table.Column<long>(type: "bigint", nullable: true),
                    updated_user_id = table.Column<long>(type: "bigint", nullable: true),
                    created_datetime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_datetime = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_m_subjects", x => x.id);
                    table.ForeignKey(
                        name: "FK_m_subjects_m_grades_grade_id",
                        column: x => x.grade_id,
                        principalTable: "m_grades",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "m_marking_rules",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    subject_id = table.Column<long>(type: "bigint", nullable: false),
                    question_type = table.Column<short>(type: "smallint", nullable: false),
                    marks_per_question = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    negative_marks = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    min_questions = table.Column<int>(type: "int", nullable: false),
                    max_questions = table.Column<int>(type: "int", nullable: false),
                    difficulty = table.Column<short>(type: "smallint", nullable: false),
                    rule_name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    description = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    created_user_id = table.Column<long>(type: "bigint", nullable: true),
                    updated_user_id = table.Column<long>(type: "bigint", nullable: true),
                    created_datetime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_datetime = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_m_marking_rules", x => x.id);
                    table.ForeignKey(
                        name: "FK_m_marking_rules_m_subjects_subject_id",
                        column: x => x.subject_id,
                        principalTable: "m_subjects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "m_questions",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    subject_id = table.Column<long>(type: "bigint", nullable: false),
                    grade_id = table.Column<long>(type: "bigint", nullable: false),
                    question_type = table.Column<short>(type: "smallint", nullable: false),
                    question_text = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    question_html = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    image_url = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    hint = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    explanation = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    difficulty = table.Column<short>(type: "smallint", nullable: false),
                    default_marks = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    negative_marks = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    eco_table_json = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    tags_json = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_user_id = table.Column<long>(type: "bigint", nullable: true),
                    updated_user_id = table.Column<long>(type: "bigint", nullable: true),
                    created_datetime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_datetime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    m_admin_userid = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_m_questions", x => x.id);
                    table.ForeignKey(
                        name: "FK_m_questions_m_admin_users_m_admin_userid",
                        column: x => x.m_admin_userid,
                        principalTable: "m_admin_users",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_m_questions_m_grades_grade_id",
                        column: x => x.grade_id,
                        principalTable: "m_grades",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_m_questions_m_subjects_subject_id",
                        column: x => x.subject_id,
                        principalTable: "m_subjects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_exams",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    exam_code = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    title = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    subject_id = table.Column<long>(type: "bigint", nullable: false),
                    grade_id = table.Column<long>(type: "bigint", nullable: false),
                    total_questions = table.Column<int>(type: "int", nullable: false),
                    duration_minutes = table.Column<int>(type: "int", nullable: false),
                    total_marks = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    pass_marks = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    description = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    exam_config_json = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    created_user_id = table.Column<long>(type: "bigint", nullable: true),
                    updated_user_id = table.Column<long>(type: "bigint", nullable: true),
                    created_datetime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_datetime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    m_admin_userid = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_exams", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_exams_m_admin_users_m_admin_userid",
                        column: x => x.m_admin_userid,
                        principalTable: "m_admin_users",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_t_exams_m_grades_grade_id",
                        column: x => x.grade_id,
                        principalTable: "m_grades",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_exams_m_subjects_subject_id",
                        column: x => x.subject_id,
                        principalTable: "m_subjects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "m_answer_options",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    question_id = table.Column<long>(type: "bigint", nullable: false),
                    option_text = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    option_html = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    option_image_url = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_correct = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    marks_allocated = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    sort_order = table.Column<int>(type: "int", nullable: false),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    created_datetime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_datetime = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_m_answer_options", x => x.id);
                    table.ForeignKey(
                        name: "FK_m_answer_options_m_questions_question_id",
                        column: x => x.question_id,
                        principalTable: "m_questions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_exam_questions",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    exam_id = table.Column<long>(type: "bigint", nullable: false),
                    question_id = table.Column<long>(type: "bigint", nullable: false),
                    question_number = table.Column<int>(type: "int", nullable: false),
                    marks_allocated = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    section_name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    created_datetime = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_exam_questions", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_exam_questions_m_questions_question_id",
                        column: x => x.question_id,
                        principalTable: "m_questions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_exam_questions_t_exams_exam_id",
                        column: x => x.exam_id,
                        principalTable: "t_exams",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_m_admin_users_email",
                table: "m_admin_users",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_m_admin_users_username",
                table: "m_admin_users",
                column: "username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_m_answer_options_question_id",
                table: "m_answer_options",
                column: "question_id");

            migrationBuilder.CreateIndex(
                name: "IX_m_grades_name",
                table: "m_grades",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_m_marking_rules_subject_id",
                table: "m_marking_rules",
                column: "subject_id");

            migrationBuilder.CreateIndex(
                name: "IX_m_questions_grade_id",
                table: "m_questions",
                column: "grade_id");

            migrationBuilder.CreateIndex(
                name: "IX_m_questions_m_admin_userid",
                table: "m_questions",
                column: "m_admin_userid");

            migrationBuilder.CreateIndex(
                name: "IX_m_questions_subject_id",
                table: "m_questions",
                column: "subject_id");

            migrationBuilder.CreateIndex(
                name: "IX_m_subjects_code",
                table: "m_subjects",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_m_subjects_grade_id",
                table: "m_subjects",
                column: "grade_id");

            migrationBuilder.CreateIndex(
                name: "IX_m_tokens_session_token",
                table: "m_tokens",
                column: "session_token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_m_tokens_user_id",
                table: "m_tokens",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_exam_questions_exam_id",
                table: "t_exam_questions",
                column: "exam_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_exam_questions_question_id",
                table: "t_exam_questions",
                column: "question_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_exams_exam_code",
                table: "t_exams",
                column: "exam_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_t_exams_grade_id",
                table: "t_exams",
                column: "grade_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_exams_m_admin_userid",
                table: "t_exams",
                column: "m_admin_userid");

            migrationBuilder.CreateIndex(
                name: "IX_t_exams_subject_id",
                table: "t_exams",
                column: "subject_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "m_answer_options");

            migrationBuilder.DropTable(
                name: "m_marking_rules");

            migrationBuilder.DropTable(
                name: "m_tokens");

            migrationBuilder.DropTable(
                name: "t_exam_questions");

            migrationBuilder.DropTable(
                name: "m_questions");

            migrationBuilder.DropTable(
                name: "t_exams");

            migrationBuilder.DropTable(
                name: "m_admin_users");

            migrationBuilder.DropTable(
                name: "m_subjects");

            migrationBuilder.DropTable(
                name: "m_grades");
        }
    }
}
