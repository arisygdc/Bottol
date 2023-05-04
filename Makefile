voicevox:
	docker run --rm --gpus all -p '127.0.0.1:50021:50021' voicevox/voicevox_engine:nvidia-ubuntu20.04-latest
bot:
	dotnet run --project Discord-bot
translator:
	node translator-api/index.js
	
.PHONY: voicevox bot translator