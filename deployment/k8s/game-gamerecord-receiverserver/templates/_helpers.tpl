{{/*
Expand the name of the chart.
*/}}
{{- define "game-gamerecord-receiverserver.name" -}}
{{- default .Chart.Name .Values.nameOverride | trunc 63 | trimSuffix "-" }}
{{- end }}

{{/*
Create a default fully qualified app name.
We truncate at 63 chars because some Kubernetes name fields are limited to this (by the DNS naming spec).
If release name contains chart name it will be used as a full name.
*/}}
{{- define "game-gamerecord-receiverserver.fullname" -}}
{{- if .Values.fullnameOverride }}
{{- .Values.fullnameOverride | trunc 63 | trimSuffix "-" }}
{{- else }}
{{- $name := default .Chart.Name .Values.nameOverride }}
{{- if contains $name .Release.Name }}
{{- .Release.Name | trunc 63 | trimSuffix "-" }}
{{- else }}
{{- printf "%s-%s" .Release.Name $name | trunc 63 | trimSuffix "-" }}
{{- end }}
{{- end }}
{{- end }}

{{/*
Create chart name and version as used by the chart label.
*/}}
{{- define "game-gamerecord-receiverserver.chart" -}}
{{- printf "%s-%s" .Chart.Name .Chart.Version | replace "+" "_" | trunc 63 | trimSuffix "-" }}
{{- end }}

{{/*
Common labels
*/}}
{{- define "game-gamerecord-receiverserver.labels" -}}
{{ include "game-gamerecord-receiverserver.selectorLabels" . }}
{{- end }}

{{/*
Selector labels
*/}}
{{- define "game-gamerecord-receiverserver.selectorLabels" -}}
app: {{ include "game-gamerecord-receiverserver.name" . }}
{{- end }}

{{/*
Create the name of the service account to use
*/}}
{{- define "game-gamerecord-receiverserver.serviceAccountName" -}}
{{- if .Values.serviceAccount.create }}
{{- default (include "game-gamerecord-receiverserver.fullname" .) .Values.serviceAccount.name }}
{{- else }}
{{- default "default" .Values.serviceAccount.name }}
{{- end }}
{{- end }}
