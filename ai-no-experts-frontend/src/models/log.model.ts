export type Recommendation = {
    field: string;
    justification: string;
    type: 'info' | 'warning' | 'error';
}