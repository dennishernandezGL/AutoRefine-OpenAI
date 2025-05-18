export type RecommendationType = 'error' | 'warning' | 'info';

export type Recommendation = {
    field: string;
    justification: string;
    type: RecommendationType;
}