#include <stdio.h>
#include <stdlib.h>
#include <string.h>

static int clamp(int value, int min, int max) {
    if (value < min) return min;
    if (value > max) return max;
    return value;
}

static const char* tier_for_score(int score) {
    if (score >= 80) return "Critical";
    if (score >= 60) return "High";
    if (score >= 35) return "Medium";
    return "Low";
}

static int score_criticality(int criticality) {
    switch (criticality) {
        case 3: return 30;
        case 2: return 22;
        case 1: return 12;
        default: return 5;
    }
}

static int score_compliance(int compliance) {
    switch (compliance) {
        case 2: return 30;
        case 1: return 16;
        default: return 2;
    }
}

static int score_value(double value) {
    if (value >= 1000000.0) return 15;
    if (value >= 250000.0) return 10;
    if (value >= 50000.0) return 5;
    return 1;
}

static int score_renewal_urgency(int days_until_renewal) {
    if (days_until_renewal <= 30) return 15;
    if (days_until_renewal <= 90) return 8;
    if (days_until_renewal <= 180) return 4;
    return 0;
}

int calculate_score(int criticality, int compliance, int incidents, double value, int days_until_renewal) {
    int score = 0;
    score += score_criticality(criticality);
    score += score_compliance(compliance);
    score += clamp(incidents * 5, 0, 20);
    score += score_value(value);
    score += score_renewal_urgency(days_until_renewal);
    return clamp(score, 0, 100);
}

#ifndef RISK_ENGINE_TEST
int main(int argc, char** argv) {
    if (argc != 6) {
        fprintf(stderr, "Usage: risk_engine <criticality 0-3> <compliance 0-2> <incident_count> <contract_value> <days_until_renewal>\n");
        return 2;
    }

    int criticality = atoi(argv[1]);
    int compliance = atoi(argv[2]);
    int incidents = atoi(argv[3]);
    double value = atof(argv[4]);
    int days_until_renewal = atoi(argv[5]);

    if (incidents < 0 || value < 0.0 || days_until_renewal < 0) {
        fprintf(stderr, "Incidents, contract value, and renewal days must be non-negative.\n");
        return 3;
    }

    int score = calculate_score(criticality, compliance, incidents, value, days_until_renewal);
    const char* tier = tier_for_score(score);

    printf("{\"score\":%d,\"tier\":\"%s\",\"rationale\":\"Rules score %d from criticality, compliance, incidents, value, and renewal urgency.\"}\n", score, tier, score);
    return 0;
}
#endif
