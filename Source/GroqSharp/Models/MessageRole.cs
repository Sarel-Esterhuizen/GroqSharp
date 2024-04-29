namespace GroqSharp.Models
{

    public enum MessageRole
    {
        System,    // Messages that set behavior or provide instructions

        User,      // Messages from a user

        Assistant  // Messages from the LLM (previous completions)
    }
}
