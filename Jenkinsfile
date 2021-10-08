def message = "";

pipeline {
	agent any
	
	environment {
		BuildConfiguration="Debug"
		ImageNameKickplayerReceiverServer="registry.gitlab.com/rayprosoft/turnkey-gaming-system/game-kickplayer-receiverserver"
		ImageTagUrlKickplayerReceiverServer = "https://gitlab.com/api/v4/projects/20478010/registry/repositories/2355332/tags"
        ImageKickplayerReceiverServer = "game-kickplayer-receiverserver"
	}

	stages {
		stage('Prepare') {
            steps {
                sh "echo 'Branch: $GIT_BRANCH'";
                script {
                    switch(env.GIT_BRANCH)
                    {
                        case ['origin/main','main']:
                            env.kube_ns = "prod";
                            env.k8s_config = 'stag-k8s-config';
                            break;
                        case ['origin/release','release']:
                            env.kube_ns = "staging";
                            env.k8s_config = 'stag-k8s-config';
                            break;
                        default:
                            env.kube_ns = "dev";
                            env.k8s_config = 'k8s-kube-config' 
                            break;
                    }
                }
                configFileProvider([
                    configFile(fileId: 'tgs.' + env.kube_ns , variable: 'tgsconfig')]) {
                        script {
                            def envItems = readProperties file: "$tgsconfig";
                            envItems.each { key, value -> env."tgs_${key}" = value;
                        }
                        env.TAG = sh(script: "echo `date +%Y%m%d%H%M`", returnStdout: true).trim() + '-' + env.kube_ns;
                        message += "\nBranch: ${env.GIT_BRANCH}" 
                        message += "\nTag: ${env.TAG}"
                        message += "\nEnv: ${env.kube_ns}"
                        env.commit_url = "${env.GIT_URL}".replace("git@gitlab.com:","https://gitlab.com/")
                            .replace(".git","/-/commit/") + "${env.GIT_COMMIT}"
                        message += "\n Commit: ${env.commit_url}"
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
				sh 'dotnet test -c $BuildConfiguration --logger trx --no-build'
			}
		}

		stage('Build Docker Image') {
			steps {
				sh 'dotnet build "TGS.GameProviders.KickPlayer.ReceiverServer/TGS.GameProviders.KickPlayer.ReceiverServer.csproj" -c $BuildConfiguration -t:ContainerBuild -p:DockerfileTag="$ImageNameKickplayerReceiverServer:${TAG}" --no-restore '
			}
		}

		stage('Push Docker Image') {
			steps {
				sh 'sudo docker push $ImageNameKickplayerReceiverServer:${TAG}'
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

		stage('Deploy to k8s') {
			steps {
				withKubeConfig([credentialsId: env.k8s_config]) {
					sh '''
						helm template deployment/k8s/game-kickplayer-receiverserver \
							--set image.tag=${TAG} \
							--set namespace=${kube_ns} \
                            --set connectionStrings.tgs="$tgs_tgs" \
                            --set connectionStrings.redis="$tgs_redis" \
                            --set connectionStrings.mongo="$tgs_mongo" \
                            --set nats.urls="$tgs_nats" \
							--set rabbit.url="$tgs_rabbit_url" \
                            --set rabbit.user="$tgs_rabbit_user" \
                            --set rabbit.password="$tgs_rabbit_pass" \
                            --set rabbit.vhost="$tgs_rabbit_vhost"
					'''
					sh '''
						helm template deployment/k8s/game-kickplayer-receiverserver \
							--set image.tag=${TAG} \
							--set namespace=${kube_ns} \
                            --set connectionStrings.tgs="$tgs_tgs" \
                            --set connectionStrings.redis="$tgs_redis" \
                            --set connectionStrings.mongo="$tgs_mongo" \
                            --set nats.urls="$tgs_nats" \
							--set rabbit.url="$tgs_rabbit_url" \
                            --set rabbit.user="$tgs_rabbit_user" \
                            --set rabbit.password="$tgs_rabbit_pass" \
                            --set rabbit.vhost="$tgs_rabbit_vhost" \
                            | kubectl -n ${kube_ns} apply -f -
					'''
				}
			}
		}

		stage('Add Tag on Prod') {
            when {
                expression {
                    return env.kube_ns == 'prod';
                }
            }			
            steps {	
				sshagent(['ssh-jenkins']) {
                    sh "git tag -a ${TAG} -m ${TAG}"
                    sh "git push ${GIT_URL} ${TAG} HEAD:${GIT_BRANCH}"
                }
            }
        }

        stage('Remove Image Tags') {					
            steps {	
                withCredentials([string(credentialsId: "GitLabToken", variable: "GitLabToken")]) {					
                    sh "curl --request DELETE --data \'name_regex_delete=.*\' --data \'keep_n=10\' --header \'PRIVATE-TOKEN: ${GitLabToken}\' ${ImageTagUrlKickplayerReceiverServer}"
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
