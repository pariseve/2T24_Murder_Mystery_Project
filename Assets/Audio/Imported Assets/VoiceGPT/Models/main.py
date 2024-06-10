from vgpt import tts

with open("Assets/VoiceGPT/Models/config.txt", "r") as file:
    for line in file:
        variable, value = line.split("=")
        variable = variable.strip()
        value = value.strip()
        exec(variable + " = " + value)

_tts = tts.VGPT(Model,Config,ASRModel,ASRConfig,F0Model,BERTModel,BERTConfig)
if(_enableEScale):
    _tts.inference(_text, _targetVoice, _outputPath, embedding_scale = _eScale, alpha = _alpha, beta = _beta, diffusion_steps=_steps)
else:
    _tts.inference(_text, _targetVoice, _outputPath, alpha = _alpha, beta = _beta, diffusion_steps=_steps)

