-- ============================================================
-- ExamSystem - MariaDB 12.0 DDL Schema Script
-- Database: exam_system
-- Target Engine: InnoDB (utf8mb4_unicode_ci)
-- Generated: 2026-08-20
-- Matches EF Core OnModelCreating in exam_system_entities.cs
-- ============================================================

-- ------------------------------------------------------------
-- 1. CREATE DATABASE
-- ------------------------------------------------------------
CREATE DATABASE IF NOT EXISTS `exam_system`
    DEFAULT CHARACTER SET = 'utf8mb4'
    DEFAULT COLLATE       = 'utf8mb4_unicode_ci';

USE `exam_system`;

-- ------------------------------------------------------------
-- 2. DROP TABLES (Safe re-run) - reverse FK order
-- ------------------------------------------------------------
SET FOREIGN_KEY_CHECKS = 0;

DROP TABLE IF EXISTS `t_exam_question`;
DROP TABLE IF EXISTS `m_answer_option`;
DROP TABLE IF EXISTS `t_exam`;
DROP TABLE IF EXISTS `m_marking_rule`;
DROP TABLE IF EXISTS `m_question`;
DROP TABLE IF EXISTS `m_subject`;
DROP TABLE IF EXISTS `m_grade`;
DROP TABLE IF EXISTS `m_token`;
DROP TABLE IF EXISTS `m_admin_user`;

SET FOREIGN_KEY_CHECKS = 1;

-- ============================================================
-- 3. TABLE: m_admin_user  (Admin / Teacher / Examiner accounts)
-- ============================================================
CREATE TABLE `m_admin_user`
(
    `id`                      BIGINT UNSIGNED  NOT NULL AUTO_INCREMENT
        COMMENT 'PK - Surrogate key',
    `username`                VARCHAR(100)     NOT NULL
        COMMENT 'Unique login username',
    `email`                   VARCHAR(200)          NULL
        COMMENT 'Unique email address',
    `password_hash`           TEXT             NOT NULL
        COMMENT 'BCrypt hashed password (60 chars minimum)',
    `full_name`               VARCHAR(255)          NULL
        COMMENT 'Display name',
    `phone`                   VARCHAR(50)           NULL
        COMMENT 'Contact phone number',
    `profile_image`           TEXT                  NULL
        COMMENT 'Profile image URL/path',
    `role`                    SMALLINT         NOT NULL DEFAULT 2
        COMMENT '1=SuperAdmin, 2=Admin, 3=Teacher, 4=Examiner',
    `is_active`               TINYINT(1)       NOT NULL DEFAULT 1
        COMMENT '1=enabled, 0=disabled',
    `is_deleted`              TINYINT(1)       NOT NULL DEFAULT 0
        COMMENT 'Soft-delete flag',
    `password_reset_token`    VARCHAR(255)          NULL
        COMMENT 'One-time reset token',
    `password_reset_expiry`   DATETIME              NULL
        COMMENT 'Expiration UTC of reset token',
    `created_user_id`         BIGINT UNSIGNED       NULL
        COMMENT 'FK → m_admin_user.id, user who created this row',
    `updated_user_id`         BIGINT UNSIGNED       NULL
        COMMENT 'FK → m_admin_user.id, last modifier',
    `created_datetime`        DATETIME         NOT NULL DEFAULT CURRENT_TIMESTAMP
        COMMENT 'Row created timestamp',
    `updated_datetime`        DATETIME              NULL
        COMMENT 'Row last updated timestamp (ON UPDATE trigger)',

    PRIMARY KEY (`id`),

    UNIQUE KEY `uk_m_admin_user_username` (`username`),
    UNIQUE KEY `uk_m_admin_user_email`    (`email`),

    KEY `ix_m_admin_user_role`        (`role`),
    KEY `ix_m_admin_user_is_active`   (`is_active`),
    KEY `ix_m_admin_user_is_deleted`  (`is_deleted`),
    KEY `ix_m_admin_user_created`     (`created_datetime`),
    KEY `fk_m_admin_user_created_by`  (`created_user_id`),
    KEY `fk_m_admin_user_updated_by`  (`updated_user_id`)
)
ENGINE = InnoDB
DEFAULT CHARSET = utf8mb4
COLLATE = utf8mb4_unicode_ci
COMMENT = 'Admin/Staff user accounts (super admin, admin, teacher, examiner)';

-- ------------------------------------------------------------
-- 3.1 Trigger: auto-update updated_datetime on m_admin_user
-- ------------------------------------------------------------
DROP TRIGGER IF EXISTS `trg_m_admin_user_bu`;
DELIMITER $$
CREATE TRIGGER `trg_m_admin_user_bu`
BEFORE UPDATE ON `m_admin_user`
FOR EACH ROW
BEGIN
    IF NEW.updated_datetime IS NULL OR NEW.updated_datetime = OLD.updated_datetime THEN
        SET NEW.updated_datetime = CURRENT_TIMESTAMP;
    END IF;
END$$
DELIMITER ;

-- ============================================================
-- 4. TABLE: m_grade  (Grade / Level / Class)
-- ============================================================
CREATE TABLE `m_grade`
(
    `id`                BIGINT UNSIGNED  NOT NULL AUTO_INCREMENT
        COMMENT 'PK',
    `name`              VARCHAR(200)     NOT NULL
        COMMENT 'Unique grade name, e.g. "Grade 10", "IELTS"',
    `level`             VARCHAR(100)          NULL
        COMMENT 'e.g. Primary / Middle / High / Language',
    `description`       TEXT                  NULL
        COMMENT 'Description of grade level',
    `sort_order`        INT              NOT NULL DEFAULT 0
        COMMENT 'Display sort order (ascending)',
    `is_active`         TINYINT(1)       NOT NULL DEFAULT 1
        COMMENT '1=active, 0=inactive',
    `is_deleted`        TINYINT(1)       NOT NULL DEFAULT 0
        COMMENT 'Soft-delete flag',
    `created_user_id`   BIGINT UNSIGNED       NULL
        COMMENT 'FK → m_admin_user.id',
    `updated_user_id`   BIGINT UNSIGNED       NULL
        COMMENT 'FK → m_admin_user.id',
    `created_datetime`  DATETIME         NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `updated_datetime`  DATETIME              NULL,

    PRIMARY KEY (`id`),

    UNIQUE KEY `uk_m_grade_name`      (`name`),
    KEY `ix_m_grade_sort_order`       (`sort_order`),
    KEY `ix_m_grade_is_active`        (`is_active`),
    KEY `fk_m_grade_created_by`       (`created_user_id`),
    KEY `fk_m_grade_updated_by`       (`updated_user_id`),

    CONSTRAINT `fk_m_grade_created_by`
        FOREIGN KEY (`created_user_id`) REFERENCES `m_admin_user` (`id`)
        ON DELETE SET NULL ON UPDATE CASCADE,
    CONSTRAINT `fk_m_grade_updated_by`
        FOREIGN KEY (`updated_user_id`) REFERENCES `m_admin_user` (`id`)
        ON DELETE SET NULL ON UPDATE CASCADE
)
ENGINE = InnoDB
DEFAULT CHARSET = utf8mb4
COLLATE = utf8mb4_unicode_ci
COMMENT = 'Grade / Level lookup table';

DROP TRIGGER IF EXISTS `trg_m_grade_bu`;
DELIMITER $$
CREATE TRIGGER `trg_m_grade_bu`
BEFORE UPDATE ON `m_grade`
FOR EACH ROW
BEGIN
    IF NEW.updated_datetime IS NULL OR NEW.updated_datetime = OLD.updated_datetime THEN
        SET NEW.updated_datetime = CURRENT_TIMESTAMP;
    END IF;
END$$
DELIMITER ;

-- ============================================================
-- 5. TABLE: m_subject  (Subject under a grade)
-- ============================================================
CREATE TABLE `m_subject`
(
    `id`                 BIGINT UNSIGNED  NOT NULL AUTO_INCREMENT
        COMMENT 'PK',
    `grade_id`           BIGINT UNSIGNED  NOT NULL
        COMMENT 'FK → m_grade.id (RESTRICT on delete)',
    `name`               VARCHAR(200)     NOT NULL
        COMMENT 'Subject name, e.g. "Mathematics"',
    `code`               VARCHAR(50)      NOT NULL
        COMMENT 'Unique subject code e.g. MATH-10',
    `description`        TEXT                  NULL
        COMMENT 'Curriculum description',
    `total_marks`        INT              NOT NULL DEFAULT 100
        COMMENT 'Default maximum marks for full subject exam',
    `pass_marks`         INT              NOT NULL DEFAULT 40
        COMMENT 'Default passing marks threshold',
    `duration_minutes`   INT              NOT NULL DEFAULT 120
        COMMENT 'Default exam duration (minutes)',
    `is_active`          TINYINT(1)       NOT NULL DEFAULT 1,
    `is_deleted`         TINYINT(1)       NOT NULL DEFAULT 0,
    `created_user_id`    BIGINT UNSIGNED       NULL
        COMMENT 'FK → m_admin_user.id',
    `updated_user_id`    BIGINT UNSIGNED       NULL
        COMMENT 'FK → m_admin_user.id',
    `created_datetime`   DATETIME         NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `updated_datetime`   DATETIME              NULL,

    PRIMARY KEY (`id`),

    UNIQUE KEY `uk_m_subject_code`      (`code`),
    KEY `ix_m_subject_grade`            (`grade_id`),
    KEY `ix_m_subject_is_active`        (`is_active`),
    KEY `ix_m_subject_name`             (`name`),
    KEY `fk_m_subject_created_by`       (`created_user_id`),
    KEY `fk_m_subject_updated_by`       (`updated_user_id`),

    CONSTRAINT `fk_m_subject_grade`
        FOREIGN KEY (`grade_id`) REFERENCES `m_grade` (`id`)
        ON DELETE RESTRICT ON UPDATE CASCADE,

    CONSTRAINT `fk_m_subject_created_by`
        FOREIGN KEY (`created_user_id`) REFERENCES `m_admin_user` (`id`)
        ON DELETE SET NULL ON UPDATE CASCADE,
    CONSTRAINT `fk_m_subject_updated_by`
        FOREIGN KEY (`updated_user_id`) REFERENCES `m_admin_user` (`id`)
        ON DELETE SET NULL ON UPDATE CASCADE
)
ENGINE = InnoDB
DEFAULT CHARSET = utf8mb4
COLLATE = utf8mb4_unicode_ci
COMMENT = 'Subjects (child of grade)';

DROP TRIGGER IF EXISTS `trg_m_subject_bu`;
DELIMITER $$
CREATE TRIGGER `trg_m_subject_bu`
BEFORE UPDATE ON `m_subject`
FOR EACH ROW
BEGIN
    IF NEW.updated_datetime IS NULL OR NEW.updated_datetime = OLD.updated_datetime THEN
        SET NEW.updated_datetime = CURRENT_TIMESTAMP;
    END IF;
END$$
DELIMITER ;

-- ============================================================
-- 6. TABLE: m_question  (Question bank)
-- ============================================================
CREATE TABLE `m_question`
(
    `id`                 BIGINT UNSIGNED  NOT NULL AUTO_INCREMENT
        COMMENT 'PK',
    `subject_id`         BIGINT UNSIGNED  NOT NULL
        COMMENT 'FK → m_subject.id (RESTRICT on delete)',
    `grade_id`           BIGINT UNSIGNED  NOT NULL
        COMMENT 'FK → m_grade.id (RESTRICT on delete - denormalized for fast filter)',
    `question_type`      SMALLINT         NOT NULL DEFAULT 1
        COMMENT '1=MCQ, 2=TrueFalse, 3=ShortAnswer, 4=Essay, 5=MathExpression, 6=BIO(Image), 7=ECO_Calculation, 8=FillBlank',
    `question_text`      MEDIUMTEXT       NOT NULL
        COMMENT 'Plain-text version of question (for searching/export)',
    `question_html`      MEDIUMTEXT           NULL
        COMMENT 'Rich HTML version of question (rendered in UI/exam)',
    `image_url`          TEXT                 NULL
        COMMENT 'Diagram or BIO question image URL',
    `hint`               TEXT                 NULL
        COMMENT 'Teacher-only / optional hint for students',
    `explanation`        TEXT                 NULL
        COMMENT 'Explanation shown after answer submission',
    `difficulty`         SMALLINT         NOT NULL DEFAULT 2
        COMMENT '1=Easy, 2=Medium, 3=Hard',
    `default_marks`      DECIMAL(10,2)    NOT NULL DEFAULT 1.00
        COMMENT 'Default score when used in an exam (can be overridden per exam)',
    `negative_marks`     DECIMAL(10,2)    NOT NULL DEFAULT 0.00
        COMMENT 'Penalty marks when answer is wrong',
    `is_active`          TINYINT(1)       NOT NULL DEFAULT 1,
    `is_deleted`         TINYINT(1)       NOT NULL DEFAULT 0
        COMMENT 'Soft-delete. Related answer_options are hard-deleted by Cascade.',
    `eco_table_json`     JSON                 NULL
        COMMENT 'ECO Calculation question - table data JSON structure',
    `tags_json`          JSON                 NULL
        COMMENT 'Question tag array JSON e.g. ["algebra","medium"]',
    `created_user_id`    BIGINT UNSIGNED       NULL
        COMMENT 'FK → m_admin_user.id',
    `updated_user_id`    BIGINT UNSIGNED       NULL
        COMMENT 'FK → m_admin_user.id',
    `created_datetime`   DATETIME         NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `updated_datetime`   DATETIME              NULL,

    PRIMARY KEY (`id`),

    KEY `ix_m_question_subject`          (`subject_id`),
    KEY `ix_m_question_grade`            (`grade_id`),
    KEY `ix_m_question_type`             (`question_type`),
    KEY `ix_m_question_difficulty`       (`difficulty`),
    KEY `ix_m_question_active`           (`is_active`, `is_deleted`),
    KEY `ix_m_question_composite`        (`subject_id`, `question_type`, `difficulty`, `is_deleted`),
    KEY `ix_m_question_created`          (`created_datetime`),
    KEY `fk_m_question_created_by`       (`created_user_id`),
    KEY `fk_m_question_updated_by`       (`updated_user_id`),

    CONSTRAINT `fk_m_question_subject`
        FOREIGN KEY (`subject_id`) REFERENCES `m_subject` (`id`)
        ON DELETE RESTRICT ON UPDATE CASCADE,

    CONSTRAINT `fk_m_question_grade`
        FOREIGN KEY (`grade_id`) REFERENCES `m_grade` (`id`)
        ON DELETE RESTRICT ON UPDATE CASCADE,

    CONSTRAINT `fk_m_question_created_by`
        FOREIGN KEY (`created_user_id`) REFERENCES `m_admin_user` (`id`)
        ON DELETE SET NULL ON UPDATE CASCADE,
    CONSTRAINT `fk_m_question_updated_by`
        FOREIGN KEY (`updated_user_id`) REFERENCES `m_admin_user` (`id`)
        ON DELETE SET NULL ON UPDATE CASCADE
)
ENGINE = InnoDB
DEFAULT CHARSET = utf8mb4
COLLATE = utf8mb4_unicode_ci
COMMENT = 'Question bank (MCQ, T/F, Short, Essay, MathExpr, BIO, ECO, FillBlank)';

DROP TRIGGER IF EXISTS `trg_m_question_bu`;
DELIMITER $$
CREATE TRIGGER `trg_m_question_bu`
BEFORE UPDATE ON `m_question`
FOR EACH ROW
BEGIN
    IF NEW.updated_datetime IS NULL OR NEW.updated_datetime = OLD.updated_datetime THEN
        SET NEW.updated_datetime = CURRENT_TIMESTAMP;
    END IF;
END$$
DELIMITER ;

-- ============================================================
-- 7. TABLE: m_answer_option  (Options for MCQ / T/F questions)
-- ============================================================
CREATE TABLE `m_answer_option`
(
    `id`                 BIGINT UNSIGNED  NOT NULL AUTO_INCREMENT,
    `question_id`        BIGINT UNSIGNED  NOT NULL
        COMMENT 'FK → m_question.id (CASCADE on delete)',
    `option_text`        MEDIUMTEXT           NULL
        COMMENT 'Plain-text option content',
    `option_html`        MEDIUMTEXT           NULL
        COMMENT 'Rich HTML option content',
    `option_image_url`   TEXT                 NULL
        COMMENT 'Image-based option URL',
    `is_correct`         TINYINT(1)       NOT NULL DEFAULT 0
        COMMENT '1 = This option is the correct answer (or partial-correct for multi-select).',
    `marks_allocated`    DECIMAL(10,2)    NOT NULL DEFAULT 0.00
        COMMENT 'Marks awarded if student picks this option.',
    `sort_order`         INT              NOT NULL DEFAULT 0
        COMMENT 'Display order within question (ascending).',
    `is_deleted`         TINYINT(1)       NOT NULL DEFAULT 0
        COMMENT 'Soft-delete within question options.',
    `created_datetime`   DATETIME         NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `updated_datetime`   DATETIME              NULL,

    PRIMARY KEY (`id`),

    KEY `ix_m_answer_option_question`    (`question_id`),
    KEY `ix_m_answer_option_correct`     (`question_id`, `is_correct`),
    KEY `ix_m_answer_option_sort_order`  (`question_id`, `sort_order`),

    CONSTRAINT `fk_m_answer_option_question`
        FOREIGN KEY (`question_id`) REFERENCES `m_question` (`id`)
        ON DELETE CASCADE ON UPDATE CASCADE
)
ENGINE = InnoDB
DEFAULT CHARSET = utf8mb4
COLLATE = utf8mb4_unicode_ci
COMMENT = 'Multiple choice / True-False answer options (CASCADE delete from m_question)';

DROP TRIGGER IF EXISTS `trg_m_answer_option_bu`;
DELIMITER $$
CREATE TRIGGER `trg_m_answer_option_bu`
BEFORE UPDATE ON `m_answer_option`
FOR EACH ROW
BEGIN
    IF NEW.updated_datetime IS NULL OR NEW.updated_datetime = OLD.updated_datetime THEN
        SET NEW.updated_datetime = CURRENT_TIMESTAMP;
    END IF;
END$$
DELIMITER ;

-- ============================================================
-- 8. TABLE: m_marking_rule  (Marking / Question selection rules)
-- ============================================================
CREATE TABLE `m_marking_rule`
(
    `id`                 BIGINT UNSIGNED  NOT NULL AUTO_INCREMENT,
    `subject_id`         BIGINT UNSIGNED  NOT NULL
        COMMENT 'FK → m_subject.id (CASCADE on delete)',
    `question_type`      SMALLINT         NOT NULL DEFAULT 1
        COMMENT '1..8 (same question_type enum as m_question)',
    `marks_per_question` DECIMAL(10,2)    NOT NULL DEFAULT 1.00
        COMMENT 'Default marks per question under this rule.',
    `negative_marks`     DECIMAL(10,2)    NOT NULL DEFAULT 0.00
        COMMENT 'Negative marks if wrong answer.',
    `min_questions`      INT              NOT NULL DEFAULT 1
        COMMENT 'Minimum questions to pick from this group per exam.',
    `max_questions`      INT              NOT NULL DEFAULT 10
        COMMENT 'Maximum questions to pick from this group per exam.',
    `difficulty`         SMALLINT         NOT NULL DEFAULT 2
        COMMENT 'Target difficulty filter (1, 2, or 3).',
    `rule_name`          VARCHAR(255)          NULL
        COMMENT 'Short descriptive name for this rule.',
    `description`        TEXT                  NULL
        COMMENT 'Long description.',
    `is_active`          TINYINT(1)       NOT NULL DEFAULT 1,
    `is_deleted`         TINYINT(1)       NOT NULL DEFAULT 0,
    `created_user_id`    BIGINT UNSIGNED       NULL
        COMMENT 'FK → m_admin_user.id',
    `updated_user_id`    BIGINT UNSIGNED       NULL
        COMMENT 'FK → m_admin_user.id',
    `created_datetime`   DATETIME         NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `updated_datetime`   DATETIME              NULL,

    PRIMARY KEY (`id`),

    KEY `ix_m_marking_rule_subject`      (`subject_id`),
    KEY `ix_m_marking_rule_active`       (`subject_id`, `is_active`),
    KEY `fk_m_marking_rule_created_by`   (`created_user_id`),
    KEY `fk_m_marking_rule_updated_by`   (`updated_user_id`),

    CONSTRAINT `fk_m_marking_rule_subject`
        FOREIGN KEY (`subject_id`) REFERENCES `m_subject` (`id`)
        ON DELETE CASCADE ON UPDATE CASCADE,

    CONSTRAINT `fk_m_marking_rule_created_by`
        FOREIGN KEY (`created_user_id`) REFERENCES `m_admin_user` (`id`)
        ON DELETE SET NULL ON UPDATE CASCADE,
    CONSTRAINT `fk_m_marking_rule_updated_by`
        FOREIGN KEY (`updated_user_id`) REFERENCES `m_admin_user` (`id`)
        ON DELETE SET NULL ON UPDATE CASCADE
)
ENGINE = InnoDB
DEFAULT CHARSET = utf8mb4
COLLATE = utf8mb4_unicode_ci
COMMENT = 'Marking rule / question-selection rule per subject + type + difficulty';

DROP TRIGGER IF EXISTS `trg_m_marking_rule_bu`;
DELIMITER $$
CREATE TRIGGER `trg_m_marking_rule_bu`
BEFORE UPDATE ON `m_marking_rule`
FOR EACH ROW
BEGIN
    IF NEW.updated_datetime IS NULL OR NEW.updated_datetime = OLD.updated_datetime THEN
        SET NEW.updated_datetime = CURRENT_TIMESTAMP;
    END IF;
END$$
DELIMITER ;

-- ============================================================
-- 9. TABLE: t_exam  (Exam / Test Paper header)
-- ============================================================
CREATE TABLE `t_exam`
(
    `id`                 BIGINT UNSIGNED  NOT NULL AUTO_INCREMENT,
    `exam_code`          VARCHAR(50)      NOT NULL
        COMMENT 'Unique exam code e.g. EXM-G10-MATH-001',
    `title`              VARCHAR(255)     NOT NULL
        COMMENT 'Exam display title',
    `exam_year`          VARCHAR(50)           NULL
        COMMENT 'Free-text exam year shown on paper header (any language, e.g. 2026 or ၂၀၂၆ ခုနှစ်)',
    `examination_center` VARCHAR(255)          NULL
        COMMENT 'Free-text examination center shown on paper header (any language)',
    `subject_id`         BIGINT UNSIGNED  NOT NULL
        COMMENT 'FK → m_subject.id (RESTRICT on delete)',
    `grade_id`           BIGINT UNSIGNED  NOT NULL
        COMMENT 'FK → m_grade.id (RESTRICT on delete)',
    `total_questions`    INT              NOT NULL DEFAULT 0,
    `duration_minutes`   INT              NOT NULL DEFAULT 120,
    `total_marks`        DECIMAL(10,2)    NOT NULL DEFAULT 0.00,
    `pass_marks`         DECIMAL(10,2)    NOT NULL DEFAULT 0.00,
    `description`        TEXT                  NULL,
    `exam_config_json`   JSON                 NULL
        COMMENT 'Exam-level config: shuffle, sections, show marks, back navigation, etc.',
    `is_active`          TINYINT(1)       NOT NULL DEFAULT 1,
    `is_deleted`         TINYINT(1)       NOT NULL DEFAULT 0,
    `created_user_id`    BIGINT UNSIGNED       NULL
        COMMENT 'FK → m_admin_user.id',
    `updated_user_id`    BIGINT UNSIGNED       NULL
        COMMENT 'FK → m_admin_user.id',
    `created_datetime`   DATETIME         NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `updated_datetime`   DATETIME              NULL,

    PRIMARY KEY (`id`),

    UNIQUE KEY `uk_t_exam_code`       (`exam_code`),
    KEY `ix_t_exam_subject`           (`subject_id`),
    KEY `ix_t_exam_grade`             (`grade_id`),
    KEY `ix_t_exam_active`            (`is_active`, `is_deleted`),
    KEY `fk_t_exam_created_by`        (`created_user_id`),
    KEY `fk_t_exam_updated_by`        (`updated_user_id`),

    CONSTRAINT `fk_t_exam_subject`
        FOREIGN KEY (`subject_id`) REFERENCES `m_subject` (`id`)
        ON DELETE RESTRICT ON UPDATE CASCADE,

    CONSTRAINT `fk_t_exam_grade`
        FOREIGN KEY (`grade_id`) REFERENCES `m_grade` (`id`)
        ON DELETE RESTRICT ON UPDATE CASCADE,

    CONSTRAINT `fk_t_exam_created_by`
        FOREIGN KEY (`created_user_id`) REFERENCES `m_admin_user` (`id`)
        ON DELETE SET NULL ON UPDATE CASCADE,
    CONSTRAINT `fk_t_exam_updated_by`
        FOREIGN KEY (`updated_user_id`) REFERENCES `m_admin_user` (`id`)
        ON DELETE SET NULL ON UPDATE CASCADE
)
ENGINE = InnoDB
DEFAULT CHARSET = utf8mb4
COLLATE = utf8mb4_unicode_ci
COMMENT = 'Exam / Test Paper definitions';

DROP TRIGGER IF EXISTS `trg_t_exam_bu`;
DELIMITER $$
CREATE TRIGGER `trg_t_exam_bu`
BEFORE UPDATE ON `t_exam`
FOR EACH ROW
BEGIN
    IF NEW.updated_datetime IS NULL OR NEW.updated_datetime = OLD.updated_datetime THEN
        SET NEW.updated_datetime = CURRENT_TIMESTAMP;
    END IF;
END$$
DELIMITER ;

-- ============================================================
-- 10. TABLE: t_exam_question  (Junction: exam ↔ question)
-- ============================================================
CREATE TABLE `t_exam_question`
(
    `id`                 BIGINT UNSIGNED  NOT NULL AUTO_INCREMENT,
    `exam_id`            BIGINT UNSIGNED  NOT NULL
        COMMENT 'FK → t_exam.id (CASCADE delete)',
    `question_id`        BIGINT UNSIGNED  NOT NULL
        COMMENT 'FK → m_question.id (RESTRICT delete)',
    `question_number`    INT              NOT NULL DEFAULT 0
        COMMENT 'Question position in exam paper (1-based).',
    `marks_allocated`    DECIMAL(10,2)    NOT NULL DEFAULT 0.00
        COMMENT 'Overrides m_question.default_marks for this specific exam.',
    `section_name`       VARCHAR(200)          NULL
        COMMENT 'Section label e.g. "Section A - Multiple Choice".',
    `is_deleted`         TINYINT(1)       NOT NULL DEFAULT 0,
    `created_datetime`   DATETIME         NOT NULL DEFAULT CURRENT_TIMESTAMP,

    PRIMARY KEY (`id`),

    UNIQUE KEY `uk_t_exam_question_unique_position` (`exam_id`, `question_number`),
    KEY `ix_t_exam_question_exam`         (`exam_id`),
    KEY `ix_t_exam_question_question`     (`question_id`),

    CONSTRAINT `fk_t_exam_question_exam`
        FOREIGN KEY (`exam_id`) REFERENCES `t_exam` (`id`)
        ON DELETE CASCADE ON UPDATE CASCADE,

    CONSTRAINT `fk_t_exam_question_question`
        FOREIGN KEY (`question_id`) REFERENCES `m_question` (`id`)
        ON DELETE RESTRICT ON UPDATE CASCADE
)
ENGINE = InnoDB
DEFAULT CHARSET = utf8mb4
COLLATE = utf8mb4_unicode_ci
COMMENT = 'Exam → Question association (junction table) with per-exam marks override';

-- ============================================================
-- 11. TABLE: m_token  (Session + JWT token store)
-- ============================================================
CREATE TABLE `m_token`
(
    `id`             BIGINT UNSIGNED  NOT NULL AUTO_INCREMENT,
    `user_id`        BIGINT UNSIGNED  NOT NULL
        COMMENT 'FK → m_admin_user.id (CASCADE delete)',
    `session_token`  VARCHAR(255)     NOT NULL
        COMMENT 'Unique random session token (also used as cookie value).',
    `jwt_token`      MEDIUMTEXT       NOT NULL
        COMMENT 'Signed JWT bearer token (Base64, ~1KB+).',
    `expires_at`     DATETIME         NOT NULL
        COMMENT 'Absolute expiration timestamp.',
    `created_at`     DATETIME         NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `is_revoked`     TINYINT(1)       NOT NULL DEFAULT 0
        COMMENT '1 = Invalidated on logout / password change.',
    `ip_address`     VARCHAR(64)           NULL
        COMMENT 'Client IP that created token (v4 or v6).',
    `user_agent`     VARCHAR(512)          NULL
        COMMENT 'Client User-Agent header truncated.',

    PRIMARY KEY (`id`),

    UNIQUE KEY `uk_m_token_session`    (`session_token`),
    KEY `ix_m_token_user`              (`user_id`),
    KEY `ix_m_token_expires`           (`expires_at`),
    KEY `ix_m_token_revoked`           (`user_id`, `is_revoked`),

    CONSTRAINT `fk_m_token_user`
        FOREIGN KEY (`user_id`) REFERENCES `m_admin_user` (`id`)
        ON DELETE CASCADE ON UPDATE CASCADE
)
ENGINE = InnoDB
DEFAULT CHARSET = utf8mb4
COLLATE = utf8mb4_unicode_ci
COMMENT = 'Active bearer / session tokens (revocable at logout)';

-- ============================================================
-- 12. MAINTENANCE: Cleanup event (optional, for expired tokens)
-- ============================================================
-- Requires MariaDB Event Scheduler = ON.
-- Enable once with: SET GLOBAL event_scheduler = ON;
DROP EVENT IF EXISTS `evt_purge_expired_tokens`;
DELIMITER $$
CREATE EVENT `evt_purge_expired_tokens`
ON SCHEDULE EVERY 1 DAY STARTS (CURRENT_TIMESTAMP + INTERVAL 1 DAY)
DO
BEGIN
    DELETE FROM `m_token`
    WHERE `is_revoked` = 1 OR `expires_at` < DATE_SUB(NOW(), INTERVAL 7 DAY);
END$$
DELIMITER ;

-- ============================================================
-- END OF DDL SCRIPT
-- ============================================================
-- FK DELETE Summary (matches EF Core OnModelCreating):
--   m_subject.grade_id          → m_grade           : RESTRICT
--   m_question.subject_id       → m_subject         : RESTRICT
--   m_question.grade_id         → m_grade           : RESTRICT
--   m_answer_option.question_id → m_question        : CASCADE
--   m_marking_rule.subject_id   → m_subject         : CASCADE
--   t_exam.subject_id           → m_subject         : RESTRICT
--   t_exam.grade_id             → m_grade           : RESTRICT
--   t_exam_question.exam_id     → t_exam            : CASCADE
--   t_exam_question.question_id → m_question        : RESTRICT
--   m_token.user_id             → m_admin_user      : CASCADE
--
-- Enums (matching C# constants in Constraints/ folder):
--   m_admin_user.role       : 1=SuperAdmin, 2=Admin, 3=Teacher, 4=Examiner
--   m_question.question_type : 1=MCQ, 2=TrueFalse, 3=ShortAnswer, 4=Essay,
--                              5=MathExpression, 6=BIO, 7=ECO_Calculation, 8=FillBlank
--   m_question.difficulty  : 1=Easy, 2=Medium, 3=Hard
-- ============================================================
