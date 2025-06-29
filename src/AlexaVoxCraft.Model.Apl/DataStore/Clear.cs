namespace AlexaVoxCraft.Model.Apl.DataStore;

public class Clear : DataStoreCommand
{
    public const string CommandType = "CLEAR";

    public Clear() : base(CommandType) { }
}