namespace GroqSharp.Models
{
    public enum MessageRoleType
    {
        System,    // Messages that set behavior or provide instructions

        User,      // Messages from a user

        Assistant, // Messages from the LLM (previous completions)

        Tool       // Messages that represent the output from function calls.
    }
}