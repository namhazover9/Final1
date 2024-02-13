using FinalWeb1.DataAccess.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalWeb1.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {            
        ICategoryRepository Category { get; }

        void Save();
        
    }
}
