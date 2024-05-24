using InterviewTest.Common;
using InterviewTest.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterviewTest.Data.Interfaces
{
    public interface ICountryRepository
    {
        public Task<ExecutionResult<Country?>> GetByIdAsync(long id);
        public Task<ExecutionResult<IEnumerable<Country>>> GetAsync(int page = 1, int pageSize = 10, string? name = null);
        public Task<ExecutionResult> AddAsync(Country Country);
        public Task<ExecutionResult> UpdateAsync(Country Country);
    }
}
