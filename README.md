# SayIt :loudspeaker:

[![.NET Core CI](https://github.com/pviotti/sayit/workflows/.NET%20Core%20CI/badge.svg)](https://github.com/pviotti/sayit/actions?query=workflow%3A%22.NET+Core+CI%22)

SayIt is a simple command line tool that pronounces written text.
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
and as CLR binary artifact.

## Usage

```bash
$ ./sayit --help
USAGE: sayit [--help] [--setup] [--version] [--listvoices] [--voice <de|en|es|fr|hi|it|ja|pt|ru|zh>] [--output <output>] <input>

INPUT:

    <input>               the text to be pronounced

OPTIONS:

    --setup               setup the configuration file
    --version             print sayit version
    --listvoices, -lv     list available voice shorthands, with their corresponding voice ids
    --voice, -v <de|en|es|fr|hi|it|ja|pt|ru|zh>
                          the voice shorthand, which maps to one of the available voice ids (see https://aka.ms/speech/tts-languages)
    --output, -o <output> the path of the output file
    --help                display this list of options.
```
At the first use you're required to run the setup wizard (`./sayit --setup`)
and enter the configuration parameters of your Azure Cognitive Services resource,
which are the subscription key (which you can find in the Azure portal) 
and the region identifier (see [here][region-ids]).
SayIt will store these parameters in the configuration folder of the current
user (e.g. `~/.config/` in Linux) as an [App Setting XML file][appsetting].

Currently, SayIt supports these settings:
 - languages (*voices*): English, Italian, French, German, Spanish, Hindi, Portuguese, Russian, Japanese and Chinese (Mandarin).

    > NB: some neural voices might not be supported by your Azure Cognitive Services resource,
depending on its region (see [here][region-voices]).

 - audio export formats: MP3 16Khz 32KB/s mono

 [az-sub]: https://azure.microsoft.com/en-us/free/
 [az-cs]: https://azure.microsoft.com/en-us/services/cognitive-services/speech-services/
 [az-cs-price]: https://azure.microsoft.com/en-us/pricing/details/cognitive-services/speech-services/
 [release]: https://github.com/pviotti/sayit/releases
 [appsetting]: https://docs.microsoft.com/en-us/dotnet/framework/configure-apps/file-schema/appsettings/
 [region-ids]: https://aka.ms/speech/sdkregion#speech-to-text-text-to-speech-and-translation
 [region-voices]: https://aka.ms/speech/sdkregion#text-to-speech
