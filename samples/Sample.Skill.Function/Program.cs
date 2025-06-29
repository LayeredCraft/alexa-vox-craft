using AlexaVoxCraft.MediatR.Lambda;
using AlexaVoxCraft.Model.Request;
using AlexaVoxCraft.Model.Response;
using Sample.Skill.Function;

return await LambdaHostExtensions.RunAlexaSkill<Function, SkillRequest, SkillResponse>();