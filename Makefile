CSC=mcs

.PHONY: all

all:
	$(CSC) hlt/*.cs -out:bin/MyBot.exe MyBot.cs
