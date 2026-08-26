using ExamSystem.DAO.Utilities;
using ExamSystem.DTOs.Common;
using ExamSystem.Entity;

namespace ExamSystem.DAO.Base
{
    public class BaseDao
    {
        private readonly LogUtility _logUtility;
        private readonly exam_system_entities _context;

        public BaseDao(exam_system_entities context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _logUtility = LogUtility.CreateLogUtility(httpContextAccessor);
        }

        public BaseDao(exam_system_entities context)
        {
            _context = context;
        }

        public bool PerformTransaction(Action<exam_system_entities> action)
        {
            bool success = false;
            using (var dbContextTransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    action.Invoke(_context);
                    dbContextTransaction.Commit();
                    success = true;
                }
                catch (Exception ex)
                {
                    _logUtility?.LogException(ex);
                    dbContextTransaction.Rollback();
                }
            }
            return success;
        }

        public async Task<bool> RollBackTransaction(Func<exam_system_entities, Task<bool>> action)
        {
            bool success = false;
            using (var dbContextTransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    success = await action.Invoke(_context);
                    dbContextTransaction.Commit();
                }
                catch (Exception ex)
                {
                    _logUtility?.LogException(ex);
                    dbContextTransaction.Rollback();
                }
            }
            return success;
        }

        public async Task<Response> Commit(Func<exam_system_entities, Task<Response>> action)
        {
            var response = new Response();
            using (var dbContextTransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    response = await action.Invoke(_context);
                    dbContextTransaction.Commit();
                }
                catch (Exception ex)
                {
                    _logUtility?.LogException(ex);
                    dbContextTransaction.Rollback();
                    response = Response.Error(new Error
                    {
                        Status = 409,
                        Title = "Transaction Error",
                        Detail = ex.Message
                    });
                }
            }
            return response;
        }

        public exam_system_entities GetContext()
        {
            return _context;
        }
    }
}
