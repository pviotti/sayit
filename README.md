# SayIt :loudspeaker:

[![.NET Core CI](https://github.com/pviotti/sayit/workflows/.NET%20Core%20CI/badge.svg)](https://github.com/pviotti/sayit/actions?query=workflow%3A%22.NET+Core+CI%22)

SayIt is a cross-platform command line tool that pronounces written text.
You can use it to create audio recordings of your text files or
to improve your pronunciation in a foreign language.

## Installation

SayIt uses [Azure Cognitive Services][az-cs] as backend to guarantee
optimal audio quality, so it requires a subscription to Azure, which you can get for free
[here][az-sub].
Azure Cognitive Services [free tier][az-cs-price] includes 5 text-to-speech
hours per month, which is often enough for personal use.

You can download SayIt in the [release section][release].
SayIt is currently distributed both as self-contained .NET Core executable
(which means you won't need to install the .NET Core runtime to use it)
and as framework-dependent .NET executable.

## Usage

```
$ ./sayit --help
USAGE: sayit [--help] [--version] [--setup] [--list-voices] [--list-formats]
             [--voice <voice>] [--format <format>] [--output <output>] [<input>]

INPUT:

    <input>               the text to be pronounced
                          (if missing, sayit will try to read it from stdin)

OPTIONS:

    --version             print sayit version
    --setup               setup the configuration file
    --list-voices, -lv    list the available voice shorthands 
                          with their corresponding voice ids
    --list-formats, -lf   list the available output format shorthands 
                          with their corresponding output format ids
    --voice, -v <voice>   the voice shorthand
    --format, -f <format> the audio output format shorthand
    --output, -o <output> the path of the output file
    --help                display this list of options.
```
At the first use you're required to run the setup wizard (`./sayit --setup`)
and enter the configuration parameters of your Azure Cognitive Services resource,
such as the subscription key (which you can find in the Azure portal) 
and the region identifier (see [here][region-ids]).
SayIt will store these parameters in the configuration folder of the current
user (e.g. `~/.config/` in Linux) as an [App Setting XML file][appsetting].

SayIt supports these settings:
 - [languages][voices]: English, Italian, French, German, Spanish, Hindi, Portuguese, Russian, Japanese and Chinese (Mandarin).
 - [output formats][output-formats]: 
`audio-16khz-32kbitrate-mono-mp3`, `audio-16khz-64kbitrate-mono-mp3`, `audio-16khz-128kbitrate-mono-mp3`, `audio-24khz-96kbitrate-mono-mp3`, `audio-24khz-160kbitrate-mono-mp3`, `audio-24khz-48kbitrate-mono-mp3`, `riff-8khz-16bit-mono-pcm`, `riff-16khz-16bit-mono-pcm`, `riff-24khz-16bit-mono-pcm`.

> NB: some languages and output formats might not be supported by your Azure Cognitive Services resource,
depending on its region (see [here][region-voices]).

 [az-sub]: https://azure.microsoft.com/en-us/free/
 [az-cs]: https://azure.microsoft.com/en-us/services/cognitive-services/speech-services/
 [az-cs-price]: https://azure.microsoft.com/en-us/pricing/details/cognitive-services/speech-services/
 [release]: https://github.com/pviotti/sayit/releases
 [appsetting]: https://docs.microsoft.com/en-us/dotnet/framework/configure-apps/file-schema/appsettings/
 [region-ids]: https://aka.ms/speech/sdkregion#speech-to-text-text-to-speech-and-translation
 [region-voices]: https://aka.ms/speech/sdkregion#text-to-speech
 [voices]: https://aka.ms/speech/tts-languages
 [output-formats]: https://docs.microsoft.com/en-us/azure/cognitive-services/speech-service/rest-text-to-speech#audio-outputs
