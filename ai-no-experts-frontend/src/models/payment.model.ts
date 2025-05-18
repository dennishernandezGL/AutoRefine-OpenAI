export type Payment = {
    billingAddress: string;
    billingAddress2: string;
    country: string;
    email: string;
    expirationDate: string;
    fullName: string;
    phone: string;
};

// Store sensitive fields separately with appropriate encryption
// type SensitivePaymentInfo should be handled within a secure context
export type SensitivePaymentInfo = {
    cardNumber: string;
    cvv: string;
    ssn: string;
};

// Ensure that sensitive information is securely managed
// and not exposed in logs or sent to unauthorized endpoints.