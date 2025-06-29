using AlexaVoxCraft.MediatR.Lambda;
using AlexaVoxCraft.Model.Apl;
using AlexaVoxCraft.Model.Response;
using Sample.Apl.Function;

APLSupport.Add();
return await LambdaHostExtensions.RunAlexaSkill<Function, APLSkillRequest, SkillResponse>();