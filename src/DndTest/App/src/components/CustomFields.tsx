import type { CustomField as CustomFieldModel } from "@/hooks/api/responses";

export function CustomFields({fields}: {fields: Array<CustomFieldModel>}) {
    return (
        <table className="CustomFields">
            <tbody>
                {fields.map(f => <CustomField field={f} key={f.id}/>)}
            </tbody>
        </table>
    );
}

function CustomField({field}: {field: CustomFieldModel}) {
    return (
        <tr className="CustomField">
            <td style={{paddingRight: 30}}>{field.name}</td>
            <td>{field.valueInteger ?? field.values.join(', ')}</td>
        </tr>
    );
}