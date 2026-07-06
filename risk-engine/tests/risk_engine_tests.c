#include <assert.h>
#include <stdio.h>

#define RISK_ENGINE_TEST
#include "../src/risk_engine.c"

int main(void) {
    int critical = calculate_score(3, 2, 5, 1200000.0, 10);
    int low = calculate_score(0, 0, 0, 1000.0, 365);

    assert(critical >= 80);
    assert(low < 35);

    printf("risk_engine_tests passed\n");
    return 0;
}
