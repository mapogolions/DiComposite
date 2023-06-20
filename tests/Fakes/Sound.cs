namespace DiComposite.Tests.Fakes;

internal interface ISound
{
    string Make();
}

internal class A : ISound
{
    public string Make() => "A";
}

internal class B : ISound
{
    public string Make() => "B";
}

internal class C : ISound
{
    public string Make() => "C";
}

internal class D : ISound
{
    public string Make() => "D";
}

internal class CompositeSound : ISound
{
    private readonly IEnumerable<ISound> _sounds;

    public CompositeSound(params ISound[] sounds)
    {
        _sounds = sounds;
    }

    public string Make() => string.Join("", _sounds.Select(x => x.Make()));
}

internal interface IPronunciation
{
    string Make(ISound sound);
}

internal class PronunciationWithPause : IPronunciation
{
    public string Make(ISound sound)
    {
        return $" {sound.Make()}";
    }
}

internal class CompositeSoundWithDep : ISound
{
    private readonly IEnumerable<ISound> _sounds;
    private readonly IPronunciation _pronunciation;

    public CompositeSoundWithDep(IPronunciation pronunciation, params ISound[] sounds)
    {
        _pronunciation = pronunciation;
        _sounds = sounds;
    }

    public string Make() => string.Join("", _sounds.Select(_pronunciation.Make));
}
