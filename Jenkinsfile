def deployedComponents = ""

pipeline {
    agent any
	
	environment {
        BuildConfiguration='Debug'
        Registry = "registry.gitlab.com/rayprosoft/turnkey-gaming-system/"
	}

    stages {
        stage('Prepare') {
            steps {
                sh "echo 'Process components: $Components'";
                sh "echo 'Branch: $GIT_BRANCH'";

                script {
                    switch(env.GIT_BRANCH)
                    {
                        case 'origin/master':
                            env.kube_ns = "prod";
                            env.k8s_config = 'stag-k8s-config';
                            break;
                        case 'origin/release':
                            env.kube_ns = "staging";
                            env.k8s_config = 'stag-k8s-config';
                            break;
                        default:
                            env.kube_ns = "dev";
                            env.k8s_config='k8s-kube-config' 
                            break;
                    }
                }

                configFileProvider([
					configFile(
						fileId: 'tgs.' + env.kube_ns , 
						variable: 'tgsconfig')]) {
					script {
						def envItems = readProperties file: "$tgsconfig";

                        envItems.each { key, value -> 
                            env."tgs_${key}" = value;
                        }

                        env.TAG = env.kube_ns + '-' + sh(script: "echo `date +%Y%m%d%H%M`", returnStdout: true).trim()
					}
				}

                sh "echo tag: $TAG"
            }
        }
        stage('Build') {
            steps {
                sh 'dotnet build -c $BuildConfiguration'
            }
        }

        stage('Test') {
			steps {
				sh  "dotnet test -c $BuildConfiguration --filter Unit /p:CollectCoverage=true --logger trx --no-build"
			}
		}

		stage('Building Docker & Push Docker game-kickplayer-receiverserver image') {
            when {
                expression {
                    return checkIsRunStage("game-kickplayer-receiverserver", params['Components'])
                }
            }
            environment {
                ImageName="${Registry}game-kickplayer-receiverserver"
            }
			steps {
				sh 'dotnet build "TGS.GameProviders.KickPlayer.ReceiverServer/TGS.GameProviders.KickPlayer.ReceiverServer.csproj" -c $BuildConfiguration -t:ContainerBuild -p:DockerfileTag="$ImageName:${TAG}" --no-restore '
                sh 'docker push $ImageName:${TAG}'
			}
        }

        stage('Deploy') {
            when {
                expression {
                    return env.kube_ns == 'prod';
                }
            }

            steps {
                script {
                    input message: 'Ready to go?', ok: 'yes';
                }
                
                sh "echo start deploy"
            }
        }

        stage('Deploy game-kickplayer-receiverserver to k8s') {
            when {
                expression {
                    return checkIsRunStage("game-kickplayer-receiverserver", params['Components'])
                }
            }
            environment {
                ImageName="${Registry}game-kickplayer-receiverserver"
            }
            steps {
                withKubeConfig([credentialsId: env.k8s_config]) {
                    sh '''
                        helm template deployment/k8s/game-kickplayer-receiverserver \
                            --set image.tag=${TAG} \
                            --set namespace=${kube_ns} \
                            --set connectionStrings.tgs="$tgs_tgs" \
                            --set connectionStrings.redis="$tgs_redis" \
                            --set connectionStrings.mongo="$tgs_mongo" \
							--set rabbit.url="$tgs_rabbit_url" \
                            --set rabbit.user="$tgs_rabbit_user" \
                            --set rabbit.password="$tgs_rabbit_pass" \
                            --set rabbit.vhost="$tgs_rabbit_vhost"
                            --set nats.urls="$tgs_nats" \
                            
                        helm template deployment/k8s/game-kickplayer-receiverserver \
                            --set image.tag=${TAG} \
                            --set namespace=${kube_ns} \
                            --set connectionStrings.tgs="$tgs_tgs" \
                            --set connectionStrings.redis="$tgs_redis" \
                            --set connectionStrings.mongo="$tgs_mongo" \
							--set rabbit.url="$tgs_rabbit_url" \
                            --set rabbit.user="$tgs_rabbit_user" \
                            --set rabbit.password="$tgs_rabbit_pass" \
                            --set rabbit.vhost="$tgs_rabbit_vhost"
                            --set nats.urls="$tgs_nats" \
                        | kubectl -n ${kube_ns} apply -f -
                    '''
                }
			}
        }
	}
	
	post {
		always {
			step([$class: 'MSTestPublisher', testResultsFile:"**/*.trx", failOnError: false, keepLongStdio: true])
		}
		success {
             sendTelegram("Pipeline: ${currentBuild.fullDisplayName} OK! ${message} \n${currentBuild.getBuildCauses()[0].shortDescription}\n")
        }
        failure {
            sendTelegram("<b>Failed Pipeline:</b> ${currentBuild.fullDisplayName} Something is wrong with ${env.BUILD_URL}/console\n${currentBuild.getBuildCauses()[0].shortDescription}\n")
        }
	}
}

def checkIsRunStage(currentName, selectedNames) {
    def arr = selectedNames.split(",")
    def isRun = false

    if(arr.length > 0)
        for(i = 0; i < arr.length; i++) {
            isRun = isRun || (arr[i] == currentName)
        }

    return isRun
}

def sendTelegram(message) {
    def encodedMessage = URLEncoder.encode(message, "UTF-8")

    withCredentials([string(credentialsId: "telegramToken", variable: "TOKEN"), string(credentialsId: "telegramChatId", variable: "CHAT_ID")]) {

        response = httpRequest (consoleLogResponseBody: true,
                contentType: 'APPLICATION_JSON',
                httpMode: 'GET',
                url: "https://api.telegram.org/bot${TOKEN}/sendMessage?text=${encodedMessage}&chat_id=${CHAT_ID}",
                validResponseCodes: '200')
        return response
    }
}
