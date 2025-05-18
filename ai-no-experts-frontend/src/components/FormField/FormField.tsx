import { FormControl, InputAdornment, TextField } from '@mui/material';
import { Field, ErrorMessage } from 'formik';
import { type FunctionComponent } from 'react';
import DOMPurify from 'dompurify';

const FormField: FunctionComponent<FormFieldProps> = ({
    endAdornment,    
    fullWidth = true,
    label = '',
    name = '',
    startAdornment,
    type = 'text',
    error = false,
    ...props
}) => {
    const sanitizedStartAdornment = startAdornment ? DOMPurify.sanitize(startAdornment) : null;
    const sanitizedEndAdornment = endAdornment ? DOMPurify.sanitize(endAdornment) : null;

    return (
        <FormControl fullWidth margin="normal">
            <Field
                as={TextField}
                fullWidth
                id={name}
                name={name}
                label={label}
                type={type}
                error={error}
                InputProps={{
                    startAdornment: sanitizedStartAdornment ? <InputAdornment position="start" dangerouslySetInnerHTML={{ __html: sanitizedStartAdornment }} /> : null,
                    endAdornment: sanitizedEndAdornment ? <InputAdornment position="start" dangerouslySetInnerHTML={{ __html: sanitizedEndAdornment }} /> : null,
                }}
                {...props}
            />
            <ErrorMessage className='form-error' name={name} component="p" />
        </FormControl>
    );
}

type FormFieldProps = {
    endAdornment?: string;
    error?: boolean;
    fullWidth?: boolean,
    label: string;
    name: string;
    startAdornment?: string;
    type?: string;
    [key: string]: any;
}

export default FormField;