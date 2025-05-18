export type Payment = {
    billingAddress: string;
    billingAddress2: string;
    cardHash: string; // Store a hash of the card number rather than the full card number.
    country: string;
    email: string;
    fullName: string;
    phone: string;
    expirationDate: string; // Consider storing month and year separately after encrypting sensitive data.
    cvvHash: string; // Store a hash of the CVV.
    ssnHash: string; // Store a hash of the SSN.
}