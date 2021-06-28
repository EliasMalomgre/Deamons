package be.kdg.stemtest.viewmodel

import androidx.lifecycle.LiveData
import androidx.lifecycle.ViewModel
import be.kdg.stemtest.model.entity.AnswerOption
import be.kdg.stemtest.model.entity.CustomDebateGameResult
import be.kdg.stemtest.model.entity.GameSettings
import be.kdg.stemtest.model.repositories.ConnectionRepository
import be.kdg.stemtest.model.repositories.ResultRepository
import be.kdg.stemtest.model.repositories.StatementRepository
import javax.inject.Inject

class PieViewModel @Inject constructor(private val statementRepository: StatementRepository,
                                       private val connectionRepository: ConnectionRepository,
                                       private val resultRepository: ResultRepository) : ViewModel() {

    fun getAnswerOptions(index: Int ): LiveData<List<AnswerOption>> {
        return statementRepository.getAnswerOptions(index+1)
    }

    fun getGameSettings(): LiveData<GameSettings> {
        return connectionRepository.getGameSettings()
    }

    fun getResults(index:Int): LiveData<CustomDebateGameResult> {
        return resultRepository.getCustomDebateGameResult(index)
    }

}
