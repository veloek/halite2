CSC=mcs
HLT=./halite
CLIENT=client-tools/hlt_client/client.py
OUT=bin/MyBot.exe
OPPONENT=bin/OpponentBot.exe

.PHONY: all

all:
	$(CSC) hlt/*.cs -out:$(OUT) MyBot.cs

clean:
	rm -f $(OUT) *.log replay-* 2> /dev/null

battle:
	$(HLT) -d "240 160" "$(OUT)" "$(OPPONENT) Enemy"

gym:
	 $(CLIENT) gym -r "$(OUT)" -r "$(OPPONENT)" -b "$(HLT)" -i 100 -H 240 -W 160
