export type Payment = {
    billingAddress: string;
    billingAddress2?: string; // Optional billingAddress2
    cardNumber: string;
    country: string;
    cvv: string;
    email: string;
    expirationDate: string;
    fullName: string;
    phone: string;
};

// Removed sensitive personal data field 'ssn'.