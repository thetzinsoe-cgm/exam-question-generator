-- ============================================================
-- ExamSystem Database - Dummy Data Seed Script
-- Target: MariaDB / MySQL (Port 3308, Database: exam_system)
-- Generated: 2026-08-20
-- ============================================================

-- ------------------------------------------------------------
-- SETUP
-- ------------------------------------------------------------
SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- ------------------------------------------------------------
-- TABLE: m_admin_user
-- Roles: 1=SuperAdmin, 2=Admin, 3=Teacher, 4=Examiner
-- Password for ALL users: Admin@123
-- BCrypt hash for "Admin@123": $2a$11$xYh8KJq5sLm1vN2pQ3rS4uO5wX6yZ7aB8cD9eF0gH1iJ2kL3mN4oP5qR
-- Note: Replace hash values with real BCrypt hashes in production.
-- Using a placeholder hash valid for demo: $2a$11$000000000000000000000000000000000000000000000000000
-- For a real quick test, use: $2a$11$xYh8KJq5sLm1vN2pQ3rS4u.abcdefghijklmnopqrstuvwxyz01
-- ------------------------------------------------------------
TRUNCATE TABLE m_admin_user;

INSERT INTO m_admin_user
(id, username, email, password_hash, full_name, phone, profile_image, role, is_active, is_deleted,
 password_reset_token, password_reset_expiry, created_user_id, updated_user_id, created_datetime, updated_datetime)
VALUES
(1, 'superadmin', 'superadmin@examsystem.com', '$2a$11$xYh8KJq5sLm1vN2pQ3rS4uO5wX6yZ7aB8cD9eF0gH1iJ2kL3mN4oP5qR',
 'Super Administrator', '09111111111', NULL, 1, 1, 0, NULL, NULL, NULL, NULL, NOW(), NULL),

(2, 'admin', 'admin@examsystem.com', '$2a$11$xYh8KJq5sLm1vN2pQ3rS4uO5wX6yZ7aB8cD9eF0gH1iJ2kL3mN4oP5qR',
 'System Admin', '09222222222', NULL, 2, 1, 0, NULL, NULL, 1, 1, NOW(), NULL),

(3, 'teacher_math', 'teacher.math@examsystem.com', '$2a$11$xYh8KJq5sLm1vN2pQ3rS4uO5wX6yZ7aB8cD9eF0gH1iJ2kL3mN4oP5qR',
 'Daw Khin Mar Yee', '09333333333', NULL, 3, 1, 0, NULL, NULL, 1, 1, NOW(), NULL),

(4, 'teacher_science', 'teacher.science@examsystem.com', '$2a$11$xYh8KJq5sLm1vN2pQ3rS4uO5wX6yZ7aB8cD9eF0gH1iJ2kL3mN4oP5qR',
 'U Min Thant', '09444444444', NULL, 3, 1, 0, NULL, NULL, 1, 1, NOW(), NULL),

(5, 'teacher_english', 'teacher.english@examsystem.com', '$2a$11$xYh8KJq5sLm1vN2pQ3rS4uO5wX6yZ7aB8cD9eF0gH1iJ2kL3mN4oP5qR',
 'Daw Cho Cho Win', '09555555555', NULL, 3, 1, 0, NULL, NULL, 1, 1, NOW(), NULL),

(6, 'teacher_history', 'teacher.history@examsystem.com', '$2a$11$xYh8KJq5sLm1vN2pQ3rS4uO5wX6yZ7aB8cD9eF0gH1iJ2kL3mN4oP5qR',
 'U Kyaw Soe', '09666666666', NULL, 3, 1, 0, NULL, NULL, 1, 1, NOW(), NULL),

(7, 'examiner1', 'examiner1@examsystem.com', '$2a$11$xYh8KJq5sLm1vN2pQ3rS4uO5wX6yZ7aB8cD9eF0gH1iJ2kL3mN4oP5qR',
 'Examiner - Aye Aye', '09777777777', NULL, 4, 1, 0, NULL, NULL, 1, 1, NOW(), NULL),

(8, 'examiner2', 'examiner2@examsystem.com', '$2a$11$xYh8KJq5sLm1vN2pQ3rS4uO5wX6yZ7aB8cD9eF0gH1iJ2kL3mN4oP5qR',
 'Examiner - Bo Bo', '09888888888', NULL, 4, 1, 0, NULL, NULL, 1, 1, NOW(), NULL);

-- ------------------------------------------------------------
-- TABLE: m_grade (Grades / Levels)
-- ------------------------------------------------------------
TRUNCATE TABLE m_grade;

INSERT INTO m_grade
(id, name, level, description, sort_order, is_active, is_deleted, created_user_id, updated_user_id, created_datetime, updated_datetime)
VALUES
(1, 'Grade 5',   'Primary',     'Primary School - Standard 5',         1, 1, 0, 1, NULL, NOW(), NULL),
(2, 'Grade 6',   'Primary',     'Primary School - Standard 6',         2, 1, 0, 1, NULL, NOW(), NULL),
(3, 'Grade 7',   'Middle',      'Middle School - Standard 7',          3, 1, 0, 1, NULL, NOW(), NULL),
(4, 'Grade 8',   'Middle',      'Middle School - Standard 8',          4, 1, 0, 1, NULL, NOW(), NULL),
(5, 'Grade 9',   'Middle',      'Middle School - Standard 9',          5, 1, 0, 1, NULL, NOW(), NULL),
(6, 'Grade 10',  'High',        'High School - Standard 10',           6, 1, 0, 1, NULL, NOW(), NULL),
(7, 'Grade 11',  'High',        'High School - Standard 11',           7, 1, 0, 1, NULL, NOW(), NULL),
(8, 'IELTS',     'Language',    'International English Testing System',8, 1, 0, 1, NULL, NOW(), NULL),
(9, 'TOEFL',     'Language',    'Test of English as Foreign Language', 9, 1, 0, 1, NULL, NOW(), NULL);

-- ------------------------------------------------------------
-- TABLE: m_subject (Subjects under each Grade)
-- ------------------------------------------------------------
TRUNCATE TABLE m_subject;

INSERT INTO m_subject
(id, grade_id, name, code, description, total_marks, pass_marks, duration_minutes, is_active, is_deleted,
 created_user_id, updated_user_id, created_datetime, updated_datetime)
VALUES
-- Grade 10 subjects
(1,  6, 'Myanmar',           'MMR-10',   'Grade 10 Myanmar Special',                 100, 40, 180, 1, 0, 1, NULL, NOW(), NULL),
(2,  6, 'English',           'ENG-10',   'Grade 10 English Special',                 100, 40, 180, 1, 0, 1, NULL, NOW(), NULL),
(3,  6, 'Mathematics',       'MATH-10',  'Grade 10 Mathematics Special',             100, 40, 180, 1, 0, 1, NULL, NOW(), NULL),
(4,  6, 'Chemistry',         'CHEM-10',  'Grade 10 Chemistry Special',               100, 40, 180, 1, 0, 1, NULL, NOW(), NULL),
(5,  6, 'Physics',           'PHY-10',   'Grade 10 Physics Special',                 100, 40, 180, 1, 0, 1, NULL, NOW(), NULL),
(6,  6, 'Biology',           'BIO-10',   'Grade 10 Biology Special',                 100, 40, 180, 1, 0, 1, NULL, NOW(), NULL),
(7,  6, 'History',           'HIS-10',   'Grade 10 History Special',                 100, 40, 180, 1, 0, 1, NULL, NOW(), NULL),
(8,  6, 'Geography',         'GEO-10',   'Grade 10 Geography Special',               100, 40, 180, 1, 0, 1, NULL, NOW(), NULL),
(9,  6, 'Economics',         'ECO-10',   'Grade 10 Economics Special',               100, 40, 180, 1, 0, 1, NULL, NOW(), NULL),

-- Grade 11 subjects
(10, 7, 'Myanmar',           'MMR-11',   'Grade 11 Myanmar Special',                 100, 40, 180, 1, 0, 1, NULL, NOW(), NULL),
(11, 7, 'English',           'ENG-11',   'Grade 11 English Special',                 100, 40, 180, 1, 0, 1, NULL, NOW(), NULL),
(12, 7, 'Mathematics',       'MATH-11',  'Grade 11 Mathematics Special',             100, 40, 180, 1, 0, 1, NULL, NOW(), NULL),
(13, 7, 'Chemistry',         'CHEM-11',  'Grade 11 Chemistry Special',               100, 40, 180, 1, 0, 1, NULL, NOW(), NULL),
(14, 7, 'Physics',           'PHY-11',   'Grade 11 Physics Special',                 100, 40, 180, 1, 0, 1, NULL, NOW(), NULL),
(15, 7, 'Biology',           'BIO-11',   'Grade 11 Biology Special',                 100, 40, 180, 1, 0, 1, NULL, NOW(), NULL),
(16, 7, 'Economics',         'ECO-11',   'Grade 11 Economics Special',               100, 40, 180, 1, 0, 1, NULL, NOW(), NULL),

-- Grade 7 subjects
(17, 3, 'Myanmar',           'MMR-07',   'Grade 7 Myanmar',                          80,  32, 120, 1, 0, 1, NULL, NOW(), NULL),
(18, 3, 'English',           'ENG-07',   'Grade 7 English',                          80,  32, 120, 1, 0, 1, NULL, NOW(), NULL),
(19, 3, 'Mathematics',       'MATH-07',  'Grade 7 Mathematics',                      80,  32, 120, 1, 0, 1, NULL, NOW(), NULL),
(20, 3, 'Science',           'SCI-07',   'Grade 7 General Science',                  80,  32, 120, 1, 0, 1, NULL, NOW(), NULL),

-- IELTS
(21, 8, 'IELTS Listening',   'IELTS-L',  'IELTS Listening Module',                   40,  20, 30,  1, 0, 1, NULL, NOW(), NULL),
(22, 8, 'IELTS Reading',     'IELTS-R',  'IELTS Reading Module',                     40,  20, 60,  1, 0, 1, NULL, NOW(), NULL),
(23, 8, 'IELTS Writing',     'IELTS-W',  'IELTS Writing Module',                     40,  20, 60,  1, 0, 1, NULL, NOW(), NULL);

-- ------------------------------------------------------------
-- TABLE: m_question (Question Bank)
-- Question Types: 1=MCQ, 2=TrueFalse, 3=ShortAnswer, 4=Essay,
--                 5=MathExpr, 6=BIO(with image), 7=ECO_Calc, 8=FillBlank
-- Difficulty: 1=Easy, 2=Medium, 3=Hard
-- ------------------------------------------------------------
TRUNCATE TABLE m_answer_option;
TRUNCATE TABLE t_exam_question;
TRUNCATE TABLE m_question;

-- ============== Grade 10 - Mathematics (subject_id=3) MCQs ==============
INSERT INTO m_question
(id, subject_id, grade_id, question_type, question_text, question_html, image_url, hint, explanation,
 difficulty, default_marks, negative_marks, is_active, is_deleted, eco_table_json, tags_json,
 created_user_id, updated_user_id, created_datetime, updated_datetime)
VALUES
(1,  3, 6, 1,
 'What is the value of x in the equation 2x + 6 = 20?',
 '<p>What is the value of x in the equation <strong>2x + 6 = 20</strong>?</p>',
 NULL, 'Subtract 6 from both sides first.', '2x = 14, so x = 7.',
 1, 1.0, 0.25, 1, 0, NULL, '["algebra","linear"]', 3, NULL, NOW(), NULL),

(2,  3, 6, 1,
 'Simplify: (3^2) * (3^3) = ?',
 '<p>Simplify: <code>3<sup>2</sup> × 3<sup>3</sup></code> = ?</p>',
 NULL, 'When multiplying same bases, add exponents.', '3^(2+3) = 3^5 = 243.',
 1, 1.0, 0.25, 1, 0, NULL, '["exponent","indices"]', 3, NULL, NOW(), NULL),

(3,  3, 6, 1,
 'What is the area of a circle with radius 7 cm? (Use π = 22/7)',
 '<p>What is the area of a circle with radius <strong>7 cm</strong>? (Use π = 22/7)</p>',
 NULL, 'Area = πr²', 'Area = (22/7) × 7 × 7 = 154 cm².',
 2, 2.0, 0.5, 1, 0, NULL, '["geometry","circle","area"]', 3, NULL, NOW(), NULL),

(4,  3, 6, 1,
 'If sin θ = 3/5, what is cos θ? (θ is acute)',
 '<p>If <code>sin θ = 3/5</code>, what is <code>cos θ</code>?</p>',
 NULL, 'Use sin²θ + cos²θ = 1', 'cos θ = 4/5',
 2, 2.0, 0.5, 1, 0, NULL, '["trigonometry","identity"]', 3, NULL, NOW(), NULL),

(5,  3, 6, 1,
 'Factorize: x² - 9',
 '<p>Factorize: <code>x² - 9</code></p>',
 NULL, 'Difference of two squares: a² - b² = (a-b)(a+b)', 'x² - 9 = (x-3)(x+3)',
 1, 1.0, 0.25, 1, 0, NULL, '["algebra","factorization"]', 3, NULL, NOW(), NULL),

(6,  3, 6, 2,
 'The sum of angles in a triangle is 360 degrees.',
 '<p>The sum of angles in a triangle is <strong>360 degrees</strong>.</p>',
 NULL, 'Remember basic triangle property.', 'Sum is 180 degrees, not 360.',
 1, 1.0, 0.25, 1, 0, NULL, '["geometry","triangle"]', 3, NULL, NOW(), NULL),

(7,  3, 6, 2,
 '√2 is an irrational number.',
 '<p><code>√2</code> is an irrational number.</p>',
 NULL, 'Irrational numbers cannot be expressed as simple fractions.', '√2 = 1.41421... non-terminating, non-repeating.',
 1, 1.0, 0.25, 1, 0, NULL, '["number-system"]', 3, NULL, NOW(), NULL),

(8,  3, 6, 3,
 'Find the LCM of 12 and 18.',
 '<p>Find the <strong>LCM</strong> of 12 and 18.</p>',
 NULL, 'Use prime factorization: 12=2²·3, 18=2·3²', 'LCM = 2² × 3² = 36',
 2, 3.0, 0.0, 1, 0, NULL, '["lcm","number-system"]', 3, NULL, NOW(), NULL),

(9,  3, 6, 5,
 'Solve the quadratic equation: x² - 5x + 6 = 0',
 '<p>Solve the quadratic equation: <strong>x² - 5x + 6 = 0</strong></p>
  <p>Show your steps using the quadratic formula:
  $$x = \\frac{-b \\pm \\sqrt{b^2 - 4ac}}{2a}$$</p>',
 NULL, 'Try factorization or use quadratic formula.', '(x-2)(x-3) = 0, so x = 2 or x = 3.',
 3, 5.0, 0.0, 1, 0, NULL, '["quadratic","algebra","katex"]', 3, NULL, NOW(), NULL),

(10, 3, 6, 1,
 'What is the slope of the line passing through (2,3) and (4,9)?',
 '<p>What is the slope of the line passing through <strong>(2,3)</strong> and <strong>(4,9)</strong>?</p>',
 NULL, 'Slope = (y2 - y1) / (x2 - x1)', 'm = (9-3)/(4-2) = 6/2 = 3',
 2, 2.0, 0.5, 1, 0, NULL, '["coordinate-geometry","slope"]', 3, NULL, NOW(), NULL);

-- ============== Grade 10 - English (subject_id=2) MCQs ==============
INSERT INTO m_question
(id, subject_id, grade_id, question_type, question_text, question_html, image_url, hint, explanation,
 difficulty, default_marks, negative_marks, is_active, is_deleted, eco_table_json, tags_json,
 created_user_id, updated_user_id, created_datetime, updated_datetime)
VALUES
(11, 2, 6, 1,
 'Choose the correct form: She ___ to school every day.',
 '<p>Choose the correct form: She <u>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</u> to school every day.</p>',
 NULL, 'Look for simple present tense, third-person singular.', '"goes" is the correct 3rd person singular form.',
 1, 1.0, 0.25, 1, 0, NULL, '["grammar","tense"]', 5, NULL, NOW(), NULL),

(12, 2, 6, 1,
 'Which word is a synonym of "happy"?',
 '<p>Which word is a <strong>synonym</strong> of "happy"?</p>',
 NULL, 'Synonym means similar meaning.', '"Joyful" means feeling pleasure and happiness.',
 1, 1.0, 0.25, 1, 0, NULL, '["vocabulary","synonym"]', 5, NULL, NOW(), NULL),

(13, 2, 6, 1,
 'Identify the noun in: "The quick brown fox jumps."',
 '<p>Identify the noun in: "<em>The quick brown fox jumps.</em>"</p>',
 NULL, 'A noun is a person, place, thing, or idea.', '"fox" is the animal (thing) - a common noun.',
 1, 1.0, 0.25, 1, 0, NULL, '["grammar","parts-of-speech"]', 5, NULL, NOW(), NULL),

(14, 2, 6, 2,
 '"I have went to the market yesterday" is grammatically correct.',
 '<p>Sentence: <em>"I have went to the market yesterday"</em> - is this grammatically correct?</p>',
 NULL, 'Check the past participle with "have".', 'Incorrect. Should be "I went..." or "I have gone..." (gone = past participle of go).',
 2, 1.0, 0.25, 1, 0, NULL, '["grammar","tense","error-spotting"]', 5, NULL, NOW(), NULL),

(15, 2, 6, 8,
 'Fill in the blank: The opposite of "hot" is _____.',
 '<p>Fill in the blank: The opposite of "hot" is <u>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</u>.</p>',
 NULL, 'Think about temperature antonyms.', 'cold',
 1, 1.0, 0.0, 1, 0, NULL, '["vocabulary","antonym","fill-blank"]', 5, NULL, NOW(), NULL),

(16, 2, 6, 4,
 'Write a paragraph (100-150 words) describing your best friend.',
 '<h4>Writing Section</h4>
  <p>Write a paragraph (100-150 words) describing your best friend. Include:
  <ul>
    <li>Name and how you met</li>
    <li>3 good qualities</li>
    <li>Why he/she is special</li>
  </ul></p>',
 NULL, 'Use descriptive adjectives and examples.', 'Teacher/examiner will grade based on content, structure, grammar, and vocabulary.',
 3, 15.0, 0.0, 1, 0, NULL, '["writing","essay","descriptive"]', 5, NULL, NOW(), NULL);

-- ============== Grade 10 - Physics (subject_id=5) ==============
INSERT INTO m_question
(id, subject_id, grade_id, question_type, question_text, question_html, image_url, hint, explanation,
 difficulty, default_marks, negative_marks, is_active, is_deleted, eco_table_json, tags_json,
 created_user_id, updated_user_id, created_datetime, updated_datetime)
VALUES
(17, 5, 6, 1,
 'What is the SI unit of force?',
 '<p>What is the <strong>SI unit</strong> of force?</p>',
 NULL, 'Named after a famous scientist who described laws of motion.', 'Newton (N) = kg·m/s²',
 1, 1.0, 0.25, 1, 0, NULL, '["mechanics","units","basic"]', 4, NULL, NOW(), NULL),

(18, 5, 6, 1,
 'A car accelerates from rest at 2 m/s² for 10 s. What is its final velocity?',
 '<p>A car accelerates from rest at <strong>2 m/s²</strong> for <strong>10 s</strong>. What is its final velocity?</p>',
 NULL, 'Use: v = u + at, where u=0 (from rest)', 'v = 0 + (2)(10) = 20 m/s',
 2, 2.0, 0.5, 1, 0, NULL, '["kinematics","equation-of-motion"]', 4, NULL, NOW(), NULL),

(19, 5, 6, 1,
 'Which law states: F = ma?',
 '<p>Which law states: <code>F = m × a</code>?</p>',
 NULL, "One of Newton's three laws.", "Newton's Second Law of Motion.",
 1, 1.0, 0.25, 1, 0, NULL, '["newton","mechanics"]', 4, NULL, NOW(), NULL),

(20, 5, 6, 2,
 'Sound can travel through a vacuum.',
 '<p><strong>True or False:</strong> Sound can travel through a vacuum.</p>',
 NULL, 'Sound requires a medium (solid/liquid/gas).', 'False. No particles = no vibration propagation.',
 1, 1.0, 0.25, 1, 0, NULL, '["waves","sound"]', 4, NULL, NOW(), NULL);

-- ============== Grade 10 - Chemistry (subject_id=4) ==============
INSERT INTO m_question
(id, subject_id, grade_id, question_type, question_text, question_html, image_url, hint, explanation,
 difficulty, default_marks, negative_marks, is_active, is_deleted, eco_table_json, tags_json,
 created_user_id, updated_user_id, created_datetime, updated_datetime)
VALUES
(21, 4, 6, 1,
 'What is the chemical symbol for Gold?',
 '<p>What is the chemical symbol for <strong>Gold</strong>?</p>',
 NULL, 'From Latin "aurum".', 'Au',
 1, 1.0, 0.25, 1, 0, NULL, '["periodic-table","symbol","basic"]', 4, NULL, NOW(), NULL),

(22, 4, 6, 1,
 'How many electrons does a neutral Carbon atom have?',
 '<p>How many electrons does a <strong>neutral Carbon</strong> atom have?</p>',
 NULL, 'Atomic number of C is 6. Neutral atom: p+ = e-', '6 electrons (atomic # 6).',
 1, 1.0, 0.25, 1, 0, NULL, '["atomic-structure","carbon"]', 4, NULL, NOW(), NULL),

(23, 4, 6, 1,
 'What type of bond is formed between Na and Cl in NaCl?',
 '<p>What type of bond is formed between <strong>Na and Cl</strong> in NaCl?</p>',
 NULL, 'Metal + Non-metal typically form this bond type.', 'Ionic bond (Na⁺ and Cl⁻)',
 2, 2.0, 0.5, 1, 0, NULL, '["chemical-bond","ionic"]', 4, NULL, NOW(), NULL),

(24, 4, 6, 3,
 'Balance the equation: _H₂ + _O₂ → _H₂O',
 '<p>Balance the equation:
  <blockquote>___ H<sub>2</sub> + ___ O<sub>2</sub> → ___ H<sub>2</sub>O</blockquote>
  Write the coefficients separated by commas.</p>',
 NULL, 'Count H and O atoms on both sides.', '2, 1, 2 → 2H₂ + O₂ → 2H₂O',
 2, 3.0, 0.0, 1, 0, NULL, '["stoichiometry","balancing"]', 4, NULL, NOW(), NULL);

-- ============== Grade 10 - Biology (subject_id=6) ==============
INSERT INTO m_question
(id, subject_id, grade_id, question_type, question_text, question_html, image_url, hint, explanation,
 difficulty, default_marks, negative_marks, is_active, is_deleted, eco_table_json, tags_json,
 created_user_id, updated_user_id, created_datetime, updated_datetime)
VALUES
(25, 6, 6, 1,
 'Which organelle is known as the "powerhouse of the cell"?',
 '<p>Which organelle is known as the <strong>"powerhouse of the cell"</strong>?</p>',
 NULL, 'Produces ATP through cellular respiration.', 'Mitochondrion (singular) / Mitochondria (plural).',
 1, 1.0, 0.25, 1, 0, NULL, '["cell-biology","organelle"]', 4, NULL, NOW(), NULL),

(26, 6, 6, 1,
 'What is the main pigment in photosynthesis?',
 '<p>What is the main pigment in <strong>photosynthesis</strong>?</p>',
 NULL, 'Gives plants their green color.', 'Chlorophyll (found in chloroplasts).',
 1, 1.0, 0.25, 1, 0, NULL, '["plant-bio","photosynthesis"]', 4, NULL, NOW(), NULL),

(27, 6, 6, 6,
 '(Diagram Question) Identify the organ that pumps blood throughout the body.',
 '<p><strong>(BIO with Image)</strong> Examine the diagram of human circulatory system provided.
  <br>Which organ pumps oxygenated blood to the body?</p>',
 '/uploads/diagrams/heart-circulatory.png', 'Located slightly left-center in your chest.', 'The Heart - specifically the left ventricle pumps to systemic circulation.',
 2, 3.0, 0.5, 1, 0, NULL, '["anatomy","heart","circulatory","image"]', 4, NULL, NOW(), NULL);

-- ============== Grade 10 - Economics (subject_id=9) ECO Calculation ==============
INSERT INTO m_question
(id, subject_id, grade_id, question_type, question_text, question_html, image_url, hint, explanation,
 difficulty, default_marks, negative_marks, is_active, is_deleted, eco_table_json, tags_json,
 created_user_id, updated_user_id, created_datetime, updated_datetime)
VALUES
(28, 9, 6, 7,
 'Calculate Total Cost (TC), Total Revenue (TR), and Profit from the data below.',
 '<h4>ECO Calculation Problem</h4>
  <p>Given:
  <ul>
    <li>Quantity Produced (Q) = 100 units</li>
    <li>Price per unit (P) = $50</li>
    <li>Fixed Cost (FC) = $2,000</li>
    <li>Variable Cost per unit (VC) = $25</li>
  </ul></p>
  <p>Calculate: (a) TC, (b) TR, (c) Profit</p>',
 NULL,
 'TC = FC + (VC × Q) ; TR = P × Q ; Profit = TR - TC',
 '(a) TC = 2000 + (25×100) = 4,500<br>(b) TR = 50×100 = 5,000<br>(c) Profit = 5,000-4,500 = $500',
 2, 10.0, 0.0, 1, 0,
 '{"columns":["Particular","Formula","Value"],"rows":[["Total Cost","FC + (VC×Q)",4500],["Total Revenue","P × Q",5000],["Profit","TR - TC",500]]}',
 '["cost","revenue","profit","eco-table"]', 6, NULL, NOW(), NULL);

-- ============== Grade 10 - History (subject_id=7) ==============
INSERT INTO m_question
(id, subject_id, grade_id, question_type, question_text, question_html, image_url, hint, explanation,
 difficulty, default_marks, negative_marks, is_active, is_deleted, eco_table_json, tags_json,
 created_user_id, updated_user_id, created_datetime, updated_datetime)
VALUES
(29, 7, 6, 1,
 'In which year did World War II end?',
 '<p>In which year did <strong>World War II</strong> officially end?</p>',
 NULL, 'War ended after Japan surrendered following atomic bombs.', '1945 (Japan surrendered on Sept 2, 1945)',
 1, 1.0, 0.25, 1, 0, NULL, '["world-war","date"]', 6, NULL, NOW(), NULL),

(30, 7, 6, 1,
 'Who was the first President of the United States?',
 '<p>Who was the <strong>first President</strong> of the United States?</p>',
 NULL, "Served 1789-1797; one of the Founding Fathers.", 'George Washington',
 1, 1.0, 0.25, 1, 0, NULL, '["usa","president","leadership"]', 6, NULL, NOW(), NULL);

-- ============== Grade 11 - Mathematics (subject_id=12) ==============
INSERT INTO m_question
(id, subject_id, grade_id, question_type, question_text, question_html, image_url, hint, explanation,
 difficulty, default_marks, negative_marks, is_active, is_deleted, eco_table_json, tags_json,
 created_user_id, updated_user_id, created_datetime, updated_datetime)
VALUES
(31, 12, 7, 1,
 'Find dy/dx if y = x³ - 2x² + 5',
 '<p>Find <code>dy/dx</code> if <code>y = x³ - 2x² + 5</code>.</p>',
 NULL, 'Use power rule: d/dx(xⁿ) = n·xⁿ⁻¹', 'dy/dx = 3x² - 4x',
 2, 2.0, 0.5, 1, 0, NULL, '["calculus","differentiation","derivative"]', 3, NULL, NOW(), NULL),

(32, 12, 7, 5,
 'Evaluate: ∫(2x + 3) dx',
 '<p>Evaluate the indefinite integral:
  $$\\int (2x + 3) \\, dx$$</p>',
 NULL, 'Apply ∫xⁿ dx = xⁿ⁺¹/(n+1) + C', '∫(2x+3)dx = x² + 3x + C',
 2, 5.0, 0.0, 1, 0, NULL, '["calculus","integration","indefinite","katex"]', 3, NULL, NOW(), NULL),

(33, 12, 7, 1,
 'What is det(A) for a 2x2 matrix A = [[3,4],[2,5]]?',
 '<p>What is the determinant of matrix
  <code>A = [3 4 ; 2 5]</code>?</p>',
 NULL, 'det([[a,b],[c,d]]) = ad - bc', 'det(A) = (3)(5)-(4)(2) = 15-8 = 7',
 2, 2.0, 0.5, 1, 0, NULL, '["matrix","determinant"]', 3, NULL, NOW(), NULL);

-- ============== IELTS - Reading (subject_id=22) ==============
INSERT INTO m_question
(id, subject_id, grade_id, question_type, question_text, question_html, image_url, hint, explanation,
 difficulty, default_marks, negative_marks, is_active, is_deleted, eco_table_json, tags_json,
 created_user_id, updated_user_id, created_datetime, updated_datetime)
VALUES
(34, 22, 8, 1,
 'Passage: "Cholesterol is a waxy substance..." - Main idea is:',
 '<div style="background:#f9f9f9;padding:15px;border-left:4px solid #007bff">
  <p><em>Cholesterol is a waxy substance found in your blood.
  Your body needs cholesterol to build healthy cells, but high levels can increase your risk of heart disease.
  There are two types: LDL ("bad") and HDL ("good").</em></p>
  </div>
  <p>The main idea of the passage is:</p>',
 NULL, 'Look for the general topic sentence.', 'Cholesterol types and their health implications.',
 2, 1.0, 0.0, 1, 0, NULL, '["ielts","reading","main-idea"]', 5, NULL, NOW(), NULL);

-- ============== Grade 7 - Science (subject_id=20) ==============
INSERT INTO m_question
(id, subject_id, grade_id, question_type, question_text, question_html, image_url, hint, explanation,
 difficulty, default_marks, negative_marks, is_active, is_deleted, eco_table_json, tags_json,
 created_user_id, updated_user_id, created_datetime, updated_datetime)
VALUES
(35, 20, 3, 1,
 'Which of these is NOT a state of matter?',
 '<p>Which of these is <strong>NOT</strong> a state of matter?</p>',
 NULL, 'Three fundamental states: solid, liquid, gas.', '"Viscous" describes thickness of a fluid, not a state.',
 1, 1.0, 0.25, 1, 0, NULL, '["matter","states","basic"]', 4, NULL, NOW(), NULL),

(36, 20, 3, 2,
 'Water boils at 100 degrees Fahrenheit at sea level.',
 '<p><strong>True or False:</strong> Water boils at <em>100 degrees Fahrenheit</em> at sea level.</p>',
 NULL, 'Compare Celsius vs Fahrenheit boiling points.', 'False. Boiling point is 100°C or 212°F.',
 1, 1.0, 0.25, 1, 0, NULL, '["temperature","measurement"]', 4, NULL, NOW(), NULL);

-- ============== Grade 10 - Geography (subject_id=8) ==============
INSERT INTO m_question
(id, subject_id, grade_id, question_type, question_text, question_html, image_url, hint, explanation,
 difficulty, default_marks, negative_marks, is_active, is_deleted, eco_table_json, tags_json,
 created_user_id, updated_user_id, created_datetime, updated_datetime)
VALUES
(37, 8, 6, 1,
 'What is the capital city of Australia?',
 '<p>What is the <strong>capital city</strong> of Australia?</p>',
 NULL, "Not Sydney! It's the planned city between Sydney and Melbourne.", 'Canberra',
 1, 1.0, 0.25, 1, 0, NULL, '["capital","australia","world-geography"]', 6, NULL, NOW(), NULL),

(38, 8, 6, 1,
 'Which is the largest ocean on Earth?',
 '<p>Which is the <strong>largest ocean</strong> on Earth?</p>',
 NULL, 'Located between Americas and Asia/Australia.', 'Pacific Ocean',
 1, 1.0, 0.25, 1, 0, NULL, '["ocean","physical-geography"]', 6, NULL, NOW(), NULL),

(39, 8, 6, 8,
 'The longest river in the world is the ______ River.',
 '<p>Fill in the blank: The longest river in the world is the
  <u>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</u> River.</p>',
 NULL, 'Located in northeastern Africa, flows north to Mediterranean.', 'Nile',
 1, 1.0, 0.0, 1, 0, NULL, '["river","fill-blank","geography"]', 6, NULL, NOW(), NULL);

-- ============== Extra Grade 10 Math (Medium & Hard pool) ==============
INSERT INTO m_question
(id, subject_id, grade_id, question_type, question_text, question_html, image_url, hint, explanation,
 difficulty, default_marks, negative_marks, is_active, is_deleted, eco_table_json, tags_json,
 created_user_id, updated_user_id, created_datetime, updated_datetime)
VALUES
(40, 3, 6, 1,
 'Solve: 2^(x+1) = 32. What is x?',
 '<p>Solve: <code>2<sup>(x+1)</sup> = 32</code>. What is x?</p>',
 NULL, 'Express both sides with same base: 32 = 2⁵', '2^(x+1)=2^5 → x+1=5 → x=4',
 2, 2.0, 0.5, 1, 0, NULL, '["exponential","equation"]', 3, NULL, NOW(), NULL),

(41, 3, 6, 1,
 'Mean of: 5, 8, 10, 15, 22 = ?',
 '<p>Calculate the arithmetic mean of: <code>[5, 8, 10, 15, 22]</code></p>',
 NULL, 'Mean = Sum / Number of items', 'Sum = 60, n = 5, Mean = 12',
 1, 1.0, 0.25, 1, 0, NULL, '["statistics","mean","average"]', 3, NULL, NOW(), NULL),

(42, 3, 6, 3,
 'Find the median of: 3, 7, 9, 4, 5, 8, 6.',
 '<p>Find the median of: <code>{3, 7, 9, 4, 5, 8, 6}</code></p>',
 NULL, 'Sort first, then pick middle value.', 'Sorted: [3,4,5,6,7,8,9] → Median = 6',
 2, 3.0, 0.0, 1, 0, NULL, '["statistics","median"]', 3, NULL, NOW(), NULL);

-- ============== Grade 10 Myanmar (subject_id=1) ==============
INSERT INTO m_question
(id, subject_id, grade_id, question_type, question_text, question_html, image_url, hint, explanation,
 difficulty, default_marks, negative_marks, is_active, is_deleted, eco_table_json, tags_json,
 created_user_id, updated_user_id, created_datetime, updated_datetime)
VALUES
(43, 1, 6, 1,
 '"မြန်မာစာ၏ ဗျည်းအက္ခရာ အရေအတွက် မည်မျှရှိသနည်း။"',
 '<p><strong>မြန်မာစာ၏ ဗျည်းအက္ခရာ အရေအတွက်</strong> မည်မျှရှိသနည်း။</p>',
 NULL, 'က စ ဋ ဏ တ ထ ဒ ဓ န ပ ဖ ဗ ဘ မ ယ ရ လ ဝ သ ဟ ဠ အ - ရေတွက်ကြည့်ပါ။', '33 လုံး (က မှ အ ထိ)',
 1, 1.0, 0.25, 1, 0, NULL, '["မြန်မာ","ဗျည်း","အခြေခံ"]', 3, NULL, NOW(), NULL),

(44, 1, 6, 2,
 '"အက္ခရာ ၃ လုံးပါသော စကားလုံးမှာ အမြဲတမ်း နာမ်တစ်ခုသာဖြစ်သည်။"',
 '<p><strong>မှန် / မမှန်</strong> - "အက္ခရာ ၃ လုံးပါသော စကားလုံးမှာ အမြဲတမ်း နာမ်တစ်ခုသာဖြစ်သည်။"</p>',
 NULL, 'ဥပမာ - "သွား" သည် ကြိယာဖြစ်ပြီး ၃ လုံးသာ ပါသည်။', 'မမှန်။ "သွား" (ကြိယာ)၊ "ပင်" (အပိုဒ်) စသည့် ၃ လုံးစကားလုံးအချို့က နာမ်မဟုတ်ပါ။',
 2, 1.0, 0.25, 1, 0, NULL, '["ဝေါဟာရ","စကားလုံးအမျိုးအစား"]', 3, NULL, NOW(), NULL);

-- ------------------------------------------------------------
-- TABLE: m_answer_option (Answer Options for MCQs)
-- Question 1 (Math): 2x+6=20  Answer= x=7 (opt B)
-- ------------------------------------------------------------
INSERT INTO m_answer_option
(id, question_id, option_text, option_html, option_image_url, is_correct, marks_allocated, sort_order, is_deleted, created_datetime, updated_datetime)
VALUES
-- Q1 (id=1): Math MCQ, Correct = B (7)
(1,  1, 'x = 5',   '<p>A. x = 5</p>',   NULL, 0, 0.0, 1, 0, NOW(), NULL),
(2,  1, 'x = 7',   '<p>B. x = 7</p>',   NULL, 1, 1.0, 2, 0, NOW(), NULL),
(3,  1, 'x = 10',  '<p>C. x = 10</p>',  NULL, 0, 0.0, 3, 0, NOW(), NULL),
(4,  1, 'x = 13',  '<p>D. x = 13</p>',  NULL, 0, 0.0, 4, 0, NOW(), NULL),

-- Q2 (id=2): 3^2 * 3^3, Correct = C (243)
(5,  2, '15',         '<p>A. 15</p>',         NULL, 0, 0.0, 1, 0, NOW(), NULL),
(6,  2, '81',         '<p>B. 81</p>',         NULL, 0, 0.0, 2, 0, NOW(), NULL),
(7,  2, '243',        '<p>C. 243</p>',        NULL, 1, 1.0, 3, 0, NOW(), NULL),
(8,  2, '729',        '<p>D. 729</p>',        NULL, 0, 0.0, 4, 0, NOW(), NULL),

-- Q3 (id=3): Circle area r=7, Correct = B (154 cm²)
(9,  3, '44 cm²',     '<p>A. 44 cm²</p>',     NULL, 0, 0.0, 1, 0, NOW(), NULL),
(10, 3, '154 cm²',    '<p>B. 154 cm²</p>',    NULL, 1, 2.0, 2, 0, NOW(), NULL),
(11, 3, '308 cm²',    '<p>C. 308 cm²</p>',    NULL, 0, 0.0, 3, 0, NOW(), NULL),
(12, 3, '49 cm²',     '<p>D. 49 cm²</p>',     NULL, 0, 0.0, 4, 0, NOW(), NULL),

-- Q4 (id=4): sin θ=3/5 → cos θ, Correct = A (4/5)
(13, 4, '4/5',        '<p>A. 4/5</p>',        NULL, 1, 2.0, 1, 0, NOW(), NULL),
(14, 4, '3/4',        '<p>B. 3/4</p>',        NULL, 0, 0.0, 2, 0, NOW(), NULL),
(15, 4, '5/4',        '<p>C. 5/4</p>',        NULL, 0, 0.0, 3, 0, NOW(), NULL),
(16, 4, '5/3',        '<p>D. 5/3</p>',        NULL, 0, 0.0, 4, 0, NOW(), NULL),

-- Q5 (id=5): Factorize x²-9, Correct = A (x-3)(x+3)
(17, 5, '(x-3)(x+3)', '<p>A. (x-3)(x+3)</p>', NULL, 1, 1.0, 1, 0, NOW(), NULL),
(18, 5, '(x-9)(x+1)', '<p>B. (x-9)(x+1)</p>', NULL, 0, 0.0, 2, 0, NOW(), NULL),
(19, 5, '(x-3)²',     '<p>C. (x-3)²</p>',     NULL, 0, 0.0, 3, 0, NOW(), NULL),
(20, 5, '(x+9)(x-1)', '<p>D. (x+9)(x-1)</p>', NULL, 0, 0.0, 4, 0, NOW(), NULL),

-- Q6 (id=6): True/False - Triangle 360°, Correct = B (False)
(21, 6, 'True',       '<p>A. True</p>',       NULL, 0, 0.0, 1, 0, NOW(), NULL),
(22, 6, 'False',      '<p>B. False</p>',      NULL, 1, 1.0, 2, 0, NOW(), NULL),

-- Q7 (id=7): True/False - √2 irrational, Correct = A (True)
(23, 7, 'True',       '<p>A. True</p>',       NULL, 1, 1.0, 1, 0, NOW(), NULL),
(24, 7, 'False',      '<p>B. False</p>',      NULL, 0, 0.0, 2, 0, NOW(), NULL),

-- Q10 (id=10): Slope (2,3)-(4,9), Correct = B (3)
(25, 10, '2',          '<p>A. 2</p>',          NULL, 0, 0.0, 1, 0, NOW(), NULL),
(26, 10, '3',          '<p>B. 3</p>',          NULL, 1, 2.0, 2, 0, NOW(), NULL),
(27, 10, '6',          '<p>C. 6</p>',          NULL, 0, 0.0, 3, 0, NOW(), NULL),
(28, 10, '1/3',        '<p>D. 1/3</p>',        NULL, 0, 0.0, 4, 0, NOW(), NULL),

-- Q11 (id=11): English - She ___ to school, Correct = B (goes)
(29, 11, 'go',         '<p>A. go</p>',         NULL, 0, 0.0, 1, 0, NOW(), NULL),
(30, 11, 'goes',       '<p>B. goes</p>',       NULL, 1, 1.0, 2, 0, NOW(), NULL),
(31, 11, 'going',      '<p>C. going</p>',      NULL, 0, 0.0, 3, 0, NOW(), NULL),
(32, 11, 'gone',       '<p>D. gone</p>',       NULL, 0, 0.0, 4, 0, NOW(), NULL),

-- Q12 (id=12): Synonym of happy, Correct = A (Joyful)
(33, 12, 'Joyful',     '<p>A. Joyful</p>',     NULL, 1, 1.0, 1, 0, NOW(), NULL),
(34, 12, 'Sad',        '<p>B. Sad</p>',        NULL, 0, 0.0, 2, 0, NOW(), NULL),
(35, 12, 'Angry',      '<p>C. Angry</p>',      NULL, 0, 0.0, 3, 0, NOW(), NULL),
(36, 12, 'Tired',      '<p>D. Tired</p>',      NULL, 0, 0.0, 4, 0, NOW(), NULL),

-- Q13 (id=13): Noun in sentence, Correct = C (fox)
(37, 13, 'Quick',      '<p>A. Quick</p>',      NULL, 0, 0.0, 1, 0, NOW(), NULL),
(38, 13, 'Brown',      '<p>B. Brown</p>',      NULL, 0, 0.0, 2, 0, NOW(), NULL),
(39, 13, 'Fox',        '<p>C. Fox</p>',        NULL, 1, 1.0, 3, 0, NOW(), NULL),
(40, 13, 'Jumps',      '<p>D. Jumps</p>',      NULL, 0, 0.0, 4, 0, NOW(), NULL),

-- Q14 (id=14): True/False grammar sentence, Correct = B (False)
(41, 14, 'True',       '<p>A. True</p>',       NULL, 0, 0.0, 1, 0, NOW(), NULL),
(42, 14, 'False',      '<p>B. False</p>',      NULL, 1, 1.0, 2, 0, NOW(), NULL),

-- Q17 (id=17): Physics SI unit of force, Correct = A (Newton)
(43, 17, 'Newton (N)', '<p>A. Newton (N)</p>', NULL, 1, 1.0, 1, 0, NOW(), NULL),
(44, 17, 'Joule (J)',  '<p>B. Joule (J)</p>',  NULL, 0, 0.0, 2, 0, NOW(), NULL),
(45, 17, 'Watt (W)',   '<p>C. Watt (W)</p>',   NULL, 0, 0.0, 3, 0, NOW(), NULL),
(46, 17, 'Pascal (Pa)','<p>D. Pascal (Pa)</p>',NULL, 0, 0.0, 4, 0, NOW(), NULL),

-- Q18 (id=18): v=u+at, Correct = C (20 m/s)
(47, 18, '2 m/s',      '<p>A. 2 m/s</p>',      NULL, 0, 0.0, 1, 0, NOW(), NULL),
(48, 18, '10 m/s',     '<p>B. 10 m/s</p>',     NULL, 0, 0.0, 2, 0, NOW(), NULL),
(49, 18, '20 m/s',     '<p>C. 20 m/s</p>',     NULL, 1, 2.0, 3, 0, NOW(), NULL),
(50, 18, '5 m/s',      '<p>D. 5 m/s</p>',      NULL, 0, 0.0, 4, 0, NOW(), NULL),

-- Q19 (id=19): F=ma law, Correct = B (2nd Law)
(51, 19, "Newton's 1st Law", "<p>A. Newton's 1<sup>st</sup> Law</p>", NULL, 0, 0.0, 1, 0, NOW(), NULL),
(52, 19, "Newton's 2nd Law", "<p>B. Newton's 2<sup>nd</sup> Law</p>", NULL, 1, 1.0, 2, 0, NOW(), NULL),
(53, 19, "Newton's 3rd Law", "<p>C. Newton's 3<sup>rd</sup> Law</p>", NULL, 0, 0.0, 3, 0, NOW(), NULL),
(54, 19, "Law of Gravity",   "<p>D. Law of Gravity</p>",              NULL, 0, 0.0, 4, 0, NOW(), NULL),

-- Q20 (id=20): Sound in vacuum, Correct = B (False)
(55, 20, 'True',       '<p>A. True</p>',       NULL, 0, 0.0, 1, 0, NOW(), NULL),
(56, 20, 'False',      '<p>B. False</p>',      NULL, 1, 1.0, 2, 0, NOW(), NULL),

-- Q21 (id=21): Gold symbol, Correct = D (Au)
(57, 21, 'Gd',         '<p>A. Gd</p>',         NULL, 0, 0.0, 1, 0, NOW(), NULL),
(58, 21, 'Go',         '<p>B. Go</p>',         NULL, 0, 0.0, 2, 0, NOW(), NULL),
(59, 21, 'Ag',         '<p>C. Ag (Silver)</p>',NULL, 0, 0.0, 3, 0, NOW(), NULL),
(60, 21, 'Au',         '<p>D. Au</p>',         NULL, 1, 1.0, 4, 0, NOW(), NULL),

-- Q22 (id=22): C electrons, Correct = A (6)
(61, 22, '6',          '<p>A. 6</p>',          NULL, 1, 1.0, 1, 0, NOW(), NULL),
(62, 22, '12',         '<p>B. 12</p>',         NULL, 0, 0.0, 2, 0, NOW(), NULL),
(63, 22, '4',          '<p>C. 4</p>',          NULL, 0, 0.0, 3, 0, NOW(), NULL),
(64, 22, '8',          '<p>D. 8</p>',          NULL, 0, 0.0, 4, 0, NOW(), NULL),

-- Q23 (id=23): NaCl bond, Correct = A (Ionic)
(65, 23, 'Ionic bond',        '<p>A. Ionic bond</p>',        NULL, 1, 2.0, 1, 0, NOW(), NULL),
(66, 23, 'Covalent bond',     '<p>B. Covalent bond</p>',     NULL, 0, 0.0, 2, 0, NOW(), NULL),
(67, 23, 'Metallic bond',     '<p>C. Metallic bond</p>',     NULL, 0, 0.0, 3, 0, NOW(), NULL),
(68, 23, 'Hydrogen bond',     '<p>D. Hydrogen bond</p>',     NULL, 0, 0.0, 4, 0, NOW(), NULL),

-- Q25 (id=25): Powerhouse organelle, Correct = B (Mitochondria)
(69, 25, 'Nucleus',           '<p>A. Nucleus</p>',           NULL, 0, 0.0, 1, 0, NOW(), NULL),
(70, 25, 'Mitochondria',      '<p>B. Mitochondria</p>',      NULL, 1, 1.0, 2, 0, NOW(), NULL),
(71, 25, 'Ribosome',          '<p>C. Ribosome</p>',          NULL, 0, 0.0, 3, 0, NOW(), NULL),
(72, 25, 'Golgi apparatus',   '<p>D. Golgi apparatus</p>',   NULL, 0, 0.0, 4, 0, NOW(), NULL),

-- Q26 (id=26): Photosynthesis pigment, Correct = A (Chlorophyll)
(73, 26, 'Chlorophyll',       '<p>A. Chlorophyll</p>',       NULL, 1, 1.0, 1, 0, NOW(), NULL),
(74, 26, 'Melanin',           '<p>B. Melanin</p>',           NULL, 0, 0.0, 2, 0, NOW(), NULL),
(75, 26, 'Hemoglobin',        '<p>C. Hemoglobin</p>',        NULL, 0, 0.0, 3, 0, NOW(), NULL),
(76, 26, 'Carotene',          '<p>D. Carotene</p>',          NULL, 0, 0.0, 4, 0, NOW(), NULL),

-- Q27 (id=27): Heart pumps blood, Correct = C (Heart)
(77, 27, 'Lungs',             '<p>A. Lungs</p>',             NULL, 0, 0.0, 1, 0, NOW(), NULL),
(78, 27, 'Liver',             '<p>B. Liver</p>',             NULL, 0, 0.0, 2, 0, NOW(), NULL),
(79, 27, 'Heart',             '<p>C. Heart</p>',             NULL, 1, 3.0, 3, 0, NOW(), NULL),
(80, 27, 'Kidney',            '<p>D. Kidney</p>',            NULL, 0, 0.0, 4, 0, NOW(), NULL),

-- Q29 (id=29): WWII end year, Correct = B (1945)
(81, 29, '1918',              '<p>A. 1918</p>',              NULL, 0, 0.0, 1, 0, NOW(), NULL),
(82, 29, '1945',              '<p>B. 1945</p>',              NULL, 1, 1.0, 2, 0, NOW(), NULL),
(83, 29, '1950',              '<p>C. 1950</p>',              NULL, 0, 0.0, 3, 0, NOW(), NULL),
(84, 29, '1939',              '<p>D. 1939</p>',              NULL, 0, 0.0, 4, 0, NOW(), NULL),

-- Q30 (id=30): 1st US President, Correct = A (G. Washington)
(85, 30, 'George Washington', '<p>A. George Washington</p>', NULL, 1, 1.0, 1, 0, NOW(), NULL),
(86, 30, 'Thomas Jefferson',  '<p>B. Thomas Jefferson</p>',  NULL, 0, 0.0, 2, 0, NOW(), NULL),
(87, 30, 'Abraham Lincoln',   '<p>C. Abraham Lincoln</p>',   NULL, 0, 0.0, 3, 0, NOW(), NULL),
(88, 30, 'John Adams',        '<p>D. John Adams</p>',        NULL, 0, 0.0, 4, 0, NOW(), NULL),

-- Q31 (id=31): dy/dx of x³-2x²+5, Correct = D (3x²-4x)
(89, 31, '3x² + 4x',          '<p>A. 3x² + 4x</p>',          NULL, 0, 0.0, 1, 0, NOW(), NULL),
(90, 31, 'x² - 4x',           '<p>B. x² - 4x</p>',           NULL, 0, 0.0, 2, 0, NOW(), NULL),
(91, 31, '3x² - 2x',          '<p>C. 3x² - 2x</p>',          NULL, 0, 0.0, 3, 0, NOW(), NULL),
(92, 31, '3x² - 4x',          '<p>D. 3x² - 4x</p>',          NULL, 1, 2.0, 4, 0, NOW(), NULL),

-- Q33 (id=33): det [[3,4],[2,5]], Correct = B (7)
(93, 33, '15',                '<p>A. 15</p>',                NULL, 0, 0.0, 1, 0, NOW(), NULL),
(94, 33, '7',                 '<p>B. 7</p>',                 NULL, 1, 2.0, 2, 0, NOW(), NULL),
(95, 33, '22',                '<p>C. 22</p>',                NULL, 0, 0.0, 3, 0, NOW(), NULL),
(96, 33, '-7',                '<p>D. -7</p>',                NULL, 0, 0.0, 4, 0, NOW(), NULL),

-- Q34 (id=34): IELTS Reading main idea, Correct = C (Cholesterol types/risks)
(97, 34, 'How to cook with oil',                      '<p>A. How to cook with oil</p>',                      NULL, 0, 0.0, 1, 0, NOW(), NULL),
(98, 34, 'Liver produces all vitamins',               '<p>B. Liver produces all vitamins</p>',               NULL, 0, 0.0, 2, 0, NOW(), NULL),
(99, 34, 'Cholesterol types and health implications', '<p>C. Cholesterol types and health implications</p>', NULL, 1, 1.0, 3, 0, NOW(), NULL),
(100,34, 'Blood donation procedure',                  '<p>D. Blood donation procedure</p>',                  NULL, 0, 0.0, 4, 0, NOW(), NULL),

-- Q35 (id=35): NOT a state of matter, Correct = D (Viscous)
(101,35, 'Solid',             '<p>A. Solid</p>',             NULL, 0, 0.0, 1, 0, NOW(), NULL),
(102,35, 'Liquid',            '<p>B. Liquid</p>',            NULL, 0, 0.0, 2, 0, NOW(), NULL),
(103,35, 'Gas',               '<p>C. Gas</p>',               NULL, 0, 0.0, 3, 0, NOW(), NULL),
(104,35, 'Viscous',           '<p>D. Viscous</p>',           NULL, 1, 1.0, 4, 0, NOW(), NULL),

-- Q36 (id=36): Water boils 100°F, Correct = B (False)
(105,36, 'True',              '<p>A. True</p>',              NULL, 0, 0.0, 1, 0, NOW(), NULL),
(106,36, 'False',             '<p>B. False</p>',             NULL, 1, 1.0, 2, 0, NOW(), NULL),

-- Q37 (id=37): Australia capital, Correct = A (Canberra)
(107,37, 'Canberra',          '<p>A. Canberra</p>',          NULL, 1, 1.0, 1, 0, NOW(), NULL),
(108,37, 'Sydney',            '<p>B. Sydney</p>',            NULL, 0, 0.0, 2, 0, NOW(), NULL),
(109,37, 'Melbourne',         '<p>C. Melbourne</p>',         NULL, 0, 0.0, 3, 0, NOW(), NULL),
(110,37, 'Perth',             '<p>D. Perth</p>',             NULL, 0, 0.0, 4, 0, NOW(), NULL),

-- Q38 (id=38): Largest ocean, Correct = A (Pacific)
(111,38, 'Pacific Ocean',     '<p>A. Pacific Ocean</p>',     NULL, 1, 1.0, 1, 0, NOW(), NULL),
(112,38, 'Atlantic Ocean',    '<p>B. Atlantic Ocean</p>',    NULL, 0, 0.0, 2, 0, NOW(), NULL),
(113,38, 'Indian Ocean',      '<p>C. Indian Ocean</p>',      NULL, 0, 0.0, 3, 0, NOW(), NULL),
(114,38, 'Arctic Ocean',      '<p>D. Arctic Ocean</p>',      NULL, 0, 0.0, 4, 0, NOW(), NULL),

-- Q40 (id=40): 2^(x+1)=32, Correct = C (4)
(115,40, '2',                 '<p>A. 2</p>',                 NULL, 0, 0.0, 1, 0, NOW(), NULL),
(116,40, '3',                 '<p>B. 3</p>',                 NULL, 0, 0.0, 2, 0, NOW(), NULL),
(117,40, '4',                 '<p>C. 4</p>',                 NULL, 1, 2.0, 3, 0, NOW(), NULL),
(118,40, '5',                 '<p>D. 5</p>',                 NULL, 0, 0.0, 4, 0, NOW(), NULL),

-- Q41 (id=41): Mean of 5,8,10,15,22, Correct = B (12)
(119,41, '10',                '<p>A. 10</p>',                NULL, 0, 0.0, 1, 0, NOW(), NULL),
(120,41, '12',                '<p>B. 12</p>',                NULL, 1, 1.0, 2, 0, NOW(), NULL),
(121,41, '15',                '<p>C. 15</p>',                NULL, 0, 0.0, 3, 0, NOW(), NULL),
(122,41, '60',                '<p>D. 60 (sum)</p>',          NULL, 0, 0.0, 4, 0, NOW(), NULL),

-- Q43 (id=43): Myanmar consonants count, Correct = C (33)
(123,43, '၂၆ လုံး',           '<p>A. ၂၆ လုံး</p>',           NULL, 0, 0.0, 1, 0, NOW(), NULL),
(124,43, '၃၀ လုံး',           '<p>B. ၃၀ လုံး</p>',           NULL, 0, 0.0, 2, 0, NOW(), NULL),
(125,43, '၃၃ လုံး',           '<p>C. ၃၃ လုံး</p>',           NULL, 1, 1.0, 3, 0, NOW(), NULL),
(126,43, '၃၆ လုံး',           '<p>D. ၃၆ လုံး</p>',           NULL, 0, 0.0, 4, 0, NOW(), NULL),

-- Q44 (id=44): Myanmar T/F sentence, Correct = B (မမှန်)
(127,44, 'မှန်',              '<p>A. မှန်</p>',              NULL, 0, 0.0, 1, 0, NOW(), NULL),
(128,44, 'မမှန်',            '<p>B. မမှန်</p>',            NULL, 1, 1.0, 2, 0, NOW(), NULL);

-- ------------------------------------------------------------
-- TABLE: m_marking_rule (Marking rules per subject & type)
-- ------------------------------------------------------------
TRUNCATE TABLE m_marking_rule;

INSERT INTO m_marking_rule
(id, subject_id, question_type, marks_per_question, negative_marks, min_questions, max_questions,
 difficulty, rule_name, description, is_active, is_deleted,
 created_user_id, updated_user_id, created_datetime, updated_datetime)
VALUES
-- Grade 10 Math (id=3)
(1,  3, 1, 1.0, 0.25, 15, 30, 1,
 'G10-Math-MCQ-Easy',     'Grade 10 Math MCQ - Easy pool',              1, 0, 1, NULL, NOW(), NULL),
(2,  3, 1, 2.0, 0.5,  8,  15, 2,
 'G10-Math-MCQ-Medium',   'Grade 10 Math MCQ - Medium pool',            1, 0, 1, NULL, NOW(), NULL),
(3,  3, 2, 1.0, 0.25, 3,  6,  1,
 'G10-Math-TF',           'Grade 10 Math True/False',                   1, 0, 1, NULL, NOW(), NULL),
(4,  3, 3, 3.0, 0.0,  3,  5,  2,
 'G10-Math-Short',        'Grade 10 Math Short Answer',                 1, 0, 1, NULL, NOW(), NULL),
(5,  3, 5, 5.0, 0.0,  2,  4,  3,
 'G10-Math-MathExpr',     'Grade 10 Math KaTeX Expression Problems',    1, 0, 1, NULL, NOW(), NULL),

-- Grade 10 English (id=2)
(6,  2, 1, 1.0, 0.0,  15, 25, 1,
 'G10-Eng-MCQ-Vocab',     'Grade 10 English MCQ - Vocab/Grammar',       1, 0, 1, NULL, NOW(), NULL),
(7,  2, 2, 1.0, 0.0,  5,  10, 2,
 'G10-Eng-TF-Error',      'Grade 10 English T/F Error Spotting',        1, 0, 1, NULL, NOW(), NULL),
(8,  2, 8, 1.0, 0.0,  3,  8,  1,
 'G10-Eng-FillBlank',     'Grade 10 English Fill in the Blanks',        1, 0, 1, NULL, NOW(), NULL),
(9,  2, 4, 15.0, 0.0, 1,  2,  3,
 'G10-Eng-Essay',         'Grade 10 English Writing/Essay (graded)',    1, 0, 1, NULL, NOW(), NULL),

-- Grade 10 Physics (id=5)
(10, 5, 1, 1.0, 0.25, 10, 20, 1,
 'G10-Phy-MCQ',           'Grade 10 Physics MCQ',                       1, 0, 1, NULL, NOW(), NULL),
(11, 5, 2, 1.0, 0.25, 3,  6,  1,
 'G10-Phy-TF',            'Grade 10 Physics True/False',                1, 0, 1, NULL, NOW(), NULL),

-- Grade 10 Chemistry (id=4)
(12, 4, 1, 1.0, 0.25, 10, 20, 1,
 'G10-Chem-MCQ',          'Grade 10 Chemistry MCQ',                     1, 0, 1, NULL, NOW(), NULL),
(13, 4, 3, 3.0, 0.0,  2,  4,  2,
 'G10-Chem-Short-Balance','Grade 10 Chemistry Balancing',               1, 0, 1, NULL, NOW(), NULL),

-- Grade 10 Biology (id=6)
(14, 6, 1, 1.0, 0.25, 10, 20, 1,
 'G10-Bio-MCQ',           'Grade 10 Biology MCQ',                       1, 0, 1, NULL, NOW(), NULL),
(15, 6, 6, 3.0, 0.5,  1,  3,  2,
 'G10-Bio-Image',         'Grade 10 Biology Diagram/Image Questions',   1, 0, 1, NULL, NOW(), NULL),

-- Grade 10 Economics (id=9)
(16, 9, 7, 10.0, 0.0, 1,  2,  2,
 'G10-Eco-Calc',          'Grade 10 Economics Table Calculations',      1, 0, 1, NULL, NOW(), NULL),

-- Grade 11 Math (id=12)
(17, 12, 1, 2.0, 0.5, 10, 20, 2,
 'G11-Math-MCQ',          'Grade 11 Math MCQ (Calculus, Matrix)',       1, 0, 1, NULL, NOW(), NULL),
(18, 12, 5, 5.0, 0.0,  2,  4,  3,
 'G11-Math-Integral',     'Grade 11 Math Integral KaTeX Problems',      1, 0, 1, NULL, NOW(), NULL),

-- IELTS Reading (id=22)
(19, 22, 1, 1.0, 0.0,  20, 40, 2,
 'IELTS-R-MCQ',           'IELTS Reading - MCQ Comprehension',          1, 0, 1, NULL, NOW(), NULL),

-- Grade 10 History (id=7)
(20, 7, 1, 1.0, 0.25, 10, 20, 1,
 'G10-His-MCQ-Dates',     'Grade 10 History Dates & Leaders MCQ',       1, 0, 1, NULL, NOW(), NULL);

-- ------------------------------------------------------------
-- TABLE: t_exam (Exam Papers)
-- ------------------------------------------------------------
INSERT INTO t_exam
(id, exam_code, title, subject_id, grade_id, total_questions, duration_minutes,
 total_marks, pass_marks, exam_date, description, exam_config_json,
 is_active, is_deleted, created_user_id, updated_user_id, created_datetime, updated_datetime)
VALUES
(1, 'EXM-G10-MATH-001',
 'Grade 10 Mathematics - Mid-Term Examination (2026)',
 3, 6, 20, 120, 30.0, 12.0,
 '2026-09-15 09:00:00',
 'Covers: Algebra, Geometry, Trigonometry, Statistics. Mix of MCQ + T/F + Short Answer.',
 '{"shuffle":true,"showMarks":true,"allowBack":true,"sections":[{"name":"Section A - MCQ","count":12,"marksEach":1},{"name":"Section B - T/F","count":4,"marksEach":1},{"name":"Section C - Short","count":4,"marksEach":3}]}',
 1, 0, 1, NULL, NOW(), NULL),

(2, 'EXM-G10-ENG-001',
 'Grade 10 English - First Semester Test',
 2, 6, 25, 150, 50.0, 20.0,
 '2026-09-16 13:00:00',
 'English Grammar, Vocab, T/F, Fill-blanks and 1 Essay writing question.',
 '{"shuffle":false,"sections":[{"name":"Grammar & Vocab","count":20},{"name":"Writing","count":1,"essayMarks":30}]}',
 1, 0, 1, NULL, NOW(), NULL),

(3, 'EXM-G10-PHY-001',
 'Grade 10 Physics - Unit 1-3 Quiz',
 5, 6, 10, 45, 12.0, 5.0,
 '2026-09-10 10:30:00',
 'Physics - Mechanics and Intro Units, quick assessment.',
 NULL,
 1, 0, 4, NULL, NOW(), NULL),

(4, 'EXM-G11-MATH-001',
 'Grade 11 Mathematics - Calculus Mock Test',
 12, 7, 15, 90, 40.0, 16.0,
 '2026-10-05 09:00:00',
 'Derivatives, Integrals, Matrix & Determinants.',
 NULL,
 1, 0, 1, NULL, NOW(), NULL),

(5, 'EXM-G10-CHEM-001',
 'Grade 10 Chemistry - Practice Paper',
 4, 6, 15, 90, 25.0, 10.0,
 '2026-09-20 11:00:00',
 'Periodic Table, Bonding, Balancing Equations.',
 NULL,
 1, 0, 4, NULL, NOW(), NULL);

-- ------------------------------------------------------------
-- TABLE: t_exam_question (Questions attached to each exam)
-- ------------------------------------------------------------
INSERT INTO t_exam_question
(id, exam_id, question_id, question_number, marks_allocated, section_name, is_deleted, created_datetime)
VALUES
-- Exam 1: G10 Math (20 questions)
-- Section A: MCQ (12)
(1,  1,  1,  1,  1.0, 'Section A - Multiple Choice', 0, NOW()),
(2,  1,  2,  2,  1.0, 'Section A - Multiple Choice', 0, NOW()),
(3,  1,  5,  3,  1.0, 'Section A - Multiple Choice', 0, NOW()),
(4,  1,  3,  4,  2.0, 'Section A - Multiple Choice', 0, NOW()),
(5,  1,  4,  5,  2.0, 'Section A - Multiple Choice', 0, NOW()),
(6,  1,  10, 6,  2.0, 'Section A - Multiple Choice', 0, NOW()),
(7,  1,  40, 7,  2.0, 'Section A - Multiple Choice', 0, NOW()),
(8,  1,  41, 8,  1.0, 'Section A - Multiple Choice', 0, NOW()),
(9,  1,  33, 9,  2.0, 'Section A - Multiple Choice', 0, NOW()),
(10, 1,  31, 10, 2.0, 'Section A - Multiple Choice', 0, NOW()),
(11, 1,  32, 11, 5.0, 'Section A - Multiple Choice', 0, NOW()),
(12, 1,  42, 12, 3.0, 'Section A - Multiple Choice', 0, NOW()),
-- Section B: True False (4)
(13, 1,  6,  13, 1.0, 'Section B - True or False', 0, NOW()),
(14, 1,  7,  14, 1.0, 'Section B - True or False', 0, NOW()),
(15, 1,  20, 15, 1.0, 'Section B - True or False', 0, NOW()),
(16, 1,  36, 16, 1.0, 'Section B - True or False', 0, NOW()),
-- Section C: Short Answer (4)
(17, 1,  8,  17, 3.0, 'Section C - Short Answer',   0, NOW()),
(18, 1,  24, 18, 3.0, 'Section C - Short Answer',   0, NOW()),
(19, 1,  9,  19, 5.0, 'Section C - Short Answer',   0, NOW()),
(20, 1,  28, 20, 10.0,'Section C - Short Answer',   0, NOW()),

-- Exam 2: G10 English (25 questions)
(21, 2, 11, 1,  1.0, 'Part 1 - Grammar MCQ',   0, NOW()),
(22, 2, 12, 2,  1.0, 'Part 1 - Grammar MCQ',   0, NOW()),
(23, 2, 13, 3,  1.0, 'Part 1 - Grammar MCQ',   0, NOW()),
(24, 2, 14, 4,  1.0, 'Part 2 - Error Spotting',0, NOW()),
(25, 2, 15, 5,  1.0, 'Part 3 - Fill in Blanks',0, NOW()),
(26, 2, 34, 6,  1.0, 'Part 4 - Reading Passage', 0, NOW()),
(27, 2, 16, 25, 15.0,'Part 5 - Writing (Essay)', 0, NOW()),

-- Exam 3: G10 Physics (10 questions)
(28, 3, 17, 1, 1.0, 'MCQ', 0, NOW()),
(29, 3, 18, 2, 2.0, 'MCQ', 0, NOW()),
(30, 3, 19, 3, 1.0, 'MCQ', 0, NOW()),
(31, 3, 20, 4, 1.0, 'True/False', 0, NOW()),
(32, 3, 21, 5, 1.0, 'MCQ', 0, NOW()),
(33, 3, 22, 6, 1.0, 'MCQ', 0, NOW()),
(34, 3, 23, 7, 2.0, 'MCQ', 0, NOW()),
(35, 3, 24, 8, 3.0, 'Short Answer', 0, NOW()),
(36, 3, 25, 9, 1.0, 'MCQ', 0, NOW()),
(37, 3, 26, 10,1.0, 'MCQ', 0, NOW()),

-- Exam 4: G11 Math (15 questions)
(38, 4, 31, 1, 2.0, 'MCQ - Calculus', 0, NOW()),
(39, 4, 32, 2, 5.0, 'Math Expression', 0, NOW()),
(40, 4, 33, 3, 2.0, 'MCQ - Matrix',   0, NOW()),
(41, 4, 40, 4, 2.0, 'MCQ - Exponents',0, NOW()),
(42, 4, 41, 5, 1.0, 'MCQ - Stats',    0, NOW()),
(43, 4, 42, 6, 3.0, 'Short Answer',   0, NOW()),
(44, 4, 1,  7, 1.0, 'Review Algebra', 0, NOW()),
(45, 4, 2,  8, 1.0, 'Review Indices', 0, NOW()),
(46, 4, 3,  9, 2.0, 'Review Geometry',0, NOW()),
(47, 4, 4,  10,2.0, 'Review Trig',    0, NOW()),
(48, 4, 5,  11,1.0, 'Review Factor',  0, NOW()),
(49, 4, 6,  12,1.0, 'Review T/F Geo', 0, NOW()),
(50, 4, 7,  13,1.0, 'Review T/F Num', 0, NOW()),
(51, 4, 10, 14,2.0, 'Review Slope',   0, NOW()),
(52, 4, 9,  15,5.0, 'Math Expression - Quadratic', 0, NOW());

-- ------------------------------------------------------------
-- TABLE: m_token (Leave empty for seed; populated at runtime)
-- ------------------------------------------------------------

SET FOREIGN_KEY_CHECKS = 1;

-- ============================================================
-- END OF SEED SCRIPT
-- ============================================================
-- Quick Reference:
--   * Login users:
--     - superadmin / Admin@123  (role: Super Admin)
--     - admin      / Admin@123  (role: Admin)
--     - teacher_math / Admin@123 (role: Teacher)
--     - examiner1  / Admin@123  (role: Examiner)
--   * Grades:   9 rows  (Grade 5-11, IELTS, TOEFL)
--   * Subjects: 23 rows (covers G10, G11, G7, IELTS)
--   * Questions: 44 rows (MCQ, T/F, Short, Essay, MathExpr, BIO, ECO_Calc, FillBlank)
--   * Answer Options: 128 rows (for all MCQ/T/F style questions)
--   * Marking Rules: 20 rows
--   * Exams: 5 pre-built exam papers
--   * Exam-Question links: 52 rows
-- ============================================================
