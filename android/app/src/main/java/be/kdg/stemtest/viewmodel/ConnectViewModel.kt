package be.kdg.stemtest.viewmodel

import androidx.lifecycle.LiveData
import androidx.lifecycle.ViewModel
import be.kdg.stemtest.model.entity.GameSettings
import be.kdg.stemtest.model.entity.Identification
import be.kdg.stemtest.model.repositories.ConnectionRepository
import javax.inject.Inject

class ConnectViewModel @Inject constructor (private val connectRepository: ConnectionRepository) : ViewModel() {

    fun getGameType(): LiveData<GameSettings> {
        return connectRepository.getGameSettings()
    }

    fun makeStudentSession(sessionCode: Int): LiveData<Identification> {
        return connectRepository.createIdentification(sessionCode)
    }

    fun deleteAll(){
         connectRepository.deleteAll()
    }
}
