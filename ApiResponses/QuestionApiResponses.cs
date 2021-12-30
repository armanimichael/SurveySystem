﻿using System.Net;
using SurveySystem.Models;

namespace SurveySystem.ApiResponses;

public class QuestionApiResponses
{
    public static readonly ApiResponse NotFound = new()
    {
        Message = "Could not find any question.",
        Success = false,
        HttpStatusCode = (int)HttpStatusCode.NotFound
    };
    
    public static readonly ApiResponse GetError = new()
    {
        Message = "There was an error retriving the selected question.",
        Success = false,
        HttpStatusCode = (int)HttpStatusCode.InternalServerError,
    };
    
    public static readonly ApiResponse NotUnique = new()
    {
        Message = "There's another question with this title.",
        Success = true,
        HttpStatusCode = (int)HttpStatusCode.Conflict
    };
}