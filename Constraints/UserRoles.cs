namespace ExamSystem.Constraints
{
    public static class UserRoles
    {
        public const short SuperAdmin = 1;
        public const short Admin = 2;
        public const short Teacher = 3;
        public const short Examiner = 4;

        public const string SuperAdminName = "Super Admin";
        public const string AdminName = "Admin";
        public const string TeacherName = "Teacher";
        public const string ExaminerName = "Examiner";

        public static string GetRoleName(this short role)
        {
            return role switch
            {
                SuperAdmin => SuperAdminName,
                Admin => AdminName,
                Teacher => TeacherName,
                Examiner => ExaminerName,
                _ => AdminName
            };
        }

        public static short[] AllAdminRoles()
        {
            return new short[] { SuperAdmin, Admin };
        }

        public static short[] AllRoles()
        {
            return new short[] { SuperAdmin, Admin, Teacher, Examiner };
        }
    }
}
