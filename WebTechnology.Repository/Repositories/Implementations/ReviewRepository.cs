using Microsoft.EntityFrameworkCore;
using WebTechnology.API;
using WebTechnology.Repository.CoreHelpers.Crud;
using WebTechnology.Repository.Repositories.Interfaces;

namespace WebTechnology.Repository.Repositories.Implementations
{
    public class ReviewRepository : GenericRepository<Review>, IReviewRepository
    {
        private readonly WebTech _context;
        public ReviewRepository(WebTech context) : base(context)
        {
            _context = context;
        }
    }
} 