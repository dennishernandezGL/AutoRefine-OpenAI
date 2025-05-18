export type Recommendation = {
    field: string;
    justification: string;
    type: 'improvement' | 'bug fix' | 'feature enhancement';
}