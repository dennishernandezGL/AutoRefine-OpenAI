export type Recommendation = {
    field: string;
    justification: string;
    type: string;
    meta?: any; // Allows addition of other meta information dynamically
}