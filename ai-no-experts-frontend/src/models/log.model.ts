export interface IRecommendation {
    field: string;
    justification: string;
    type: 'enhancement' | 'bugfix' | 'refactor';
}

export type Recommendation = IRecommendation;
    field: string;
    justification: string;
    type: string;
}