﻿using Microsoft.EntityFrameworkCore;
using SurveySystem.Data;
using SurveySystem.Models;
using SurveySystem.services.UserService;

namespace SurveySystem.services.SurveyService;

public class SurveyService : ISurveyService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IUserService _userService;

    public SurveyService(ApplicationDbContext dbContext, IUserService userService)
    {
        _dbContext = dbContext;
        _userService = userService;
    }

    public async Task<IList<Survey>> Get()
    {
        return await _dbContext.Surveys.ToListAsync();
    }

    public async Task<Survey?> Get(Guid id)
    {
        return await _dbContext.Surveys.SingleOrDefaultAsync(s => s.Id.Equals(id));
    }

    public async Task<Survey?> Create(Survey survey)
    {
        if (!await IsUnique(survey))
        {
            return null;
        }

        survey.UserId = (await _userService.GetCurrentUser()).Id;
        await _dbContext.AddAsync(survey);
        await _dbContext.SaveChangesAsync();
        return survey;
    }

    private async Task<bool> IsUnique(Survey survey)
    {
        return !await _dbContext.Surveys.AnyAsync(s => s.Name == survey.Name);
    }
}