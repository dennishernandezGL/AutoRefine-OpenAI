export type RecommendationType = 'validation' | 'performance' | 'security' | 'usability';

export type Recommendation = {
    field: string;
    justification: string;
    type: RecommendationType;
}