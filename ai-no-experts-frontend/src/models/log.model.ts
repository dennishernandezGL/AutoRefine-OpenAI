enum RecommendationType {
    BugFix = "bug_fix",
    Refactoring = "refactoring",
    FeatureEnhancement = "feature_enhancement"
}

export type Recommendation = {
    field: string;
    justification: string;
    type: RecommendationType;
}