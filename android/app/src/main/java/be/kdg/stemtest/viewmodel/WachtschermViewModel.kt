package be.kdg.stemtest.viewmodel

import androidx.lifecycle.LiveData
import androidx.lifecycle.ViewModel
import be.kdg.stemtest.model.entity.GameSettings
import be.kdg.stemtest.model.entity.Identification
import be.kdg.stemtest.model.repositories.ConnectionRepository
import be.kdg.stemtest.model.repositories.StatementRepository
import javax.inject.Inject

class WachtschermViewModel @Inject constructor(private val connectionRepository: ConnectionRepository,
                                                private val statementRepository: StatementRepository) : ViewModel() {

    fun getIdentification(): LiveData<Identification>{
        return connectionRepository.getIdentification()
    }

    fun getCurrStatement(): Int {
        var currStatement = statementRepository.getIndex()
        if (currStatement!=null){
           return currStatement
        }else{
            return 0
        }

    }
    fun getGameData(): LiveData<GameSettings> {
        return connectionRepository.getGameSettings()
    }

}
