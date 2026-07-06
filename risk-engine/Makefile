CC ?= gcc
CFLAGS ?= -std=c11 -Wall -Wextra -Werror -O2

.PHONY: all test clean

all: bin/risk_engine

bin:
	mkdir -p bin

bin/risk_engine: src/risk_engine.c | bin
	$(CC) $(CFLAGS) src/risk_engine.c -o bin/risk_engine

bin/risk_engine_tests: tests/risk_engine_tests.c src/risk_engine.c | bin
	$(CC) $(CFLAGS) tests/risk_engine_tests.c -o bin/risk_engine_tests

test: bin/risk_engine_tests
	./bin/risk_engine_tests

clean:
	rm -rf bin
