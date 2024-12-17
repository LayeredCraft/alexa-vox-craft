﻿using AlexaVoxCraft.Model.Requests;

namespace AlexaVoxCraft.MediatR.Lambda.Context;

public interface ISkillContextFactory
{
    SkillContext Create(SkillRequest request);

    void Dispose(SkillContext skillContext);
}