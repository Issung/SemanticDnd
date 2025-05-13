import { useParams } from '@tanstack/react-router'

export default function DocumentView() {
  const { id } = useParams({ strict: false }) // will be typed later with route param
  return <div>Document ID: {id}</div>
}
