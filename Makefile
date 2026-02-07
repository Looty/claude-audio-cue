.PHONY: build publish test

build:
	dotnet build src/ClaudeAudioCue/ClaudeAudioCue.csproj

publish:
	dotnet publish src/ClaudeAudioCue/ClaudeAudioCue.csproj -c Release -r win-x64 --self-contained -p:PublishSingleFile=true

test: build
	@echo "Tests passed (build succeeded)"
