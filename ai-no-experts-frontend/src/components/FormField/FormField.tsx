import { FormControl, InputAdornment, TextField } from '@mui/material';
import { Field, ErrorMessage } from 'formik';
import { type FunctionComponent } from 'react';

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
    // Sanitize input values to prevent XSS
    const sanitizeInput = (input: string) => {
        return input.replace(/[&<>"]/g, function(tag) {
            const charsToReplace = { '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;' };
            return charsToReplace[tag] || tag;
        });
    };

    return (
        <FormControl fullWidth margin="normal">
            <Field
                as={TextField}
                fullWidth
                id={sanitizeInput(name)}
                name={sanitizeInput(name)}
                label={sanitizeInput(label)}
                type={type}
                error={error}
                InputProps={{
                    startAdornment: startAdornment ? <InputAdornment position="start">{sanitizeInput(startAdornment)}</InputAdornment> : null,
                    endAdornment: endAdornment ? <InputAdornment position="end">{sanitizeInput(endAdornment)}</InputAdornment> : null,
                }}
                {...props}
            />
            <ErrorMessage className='form-error' name={sanitizeInput(name)} component="p" />
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